using System.Diagnostics;
using PubNet.API.Services;
using PubNet.API.WorkerTasks;

namespace PubNet.API;

public class Worker : BackgroundService
{
    private const int MaxTries = 10;

    private readonly ILogger<Worker> _logger;
    private readonly WorkerTaskQueue _taskQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, WorkerTaskQueue taskQueue, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _taskQueue = taskQueue;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _taskQueue.Enqueue(new CleanupOldPendingArchivesTask());
        _taskQueue.Enqueue(new MissingAnalysisQueuingTask());

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!TimeSpan.TryParse(_configuration.GetRequiredSection("Worker:SleepInterval").Value!, out var sleepDuration))
        {
            _logger.LogCritical("Failed to parse Worker:SleepInterval value. Check your configuration");

            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_taskQueue.IsEmpty)
                {
                    _logger.LogInformation("No tasks queued");

                    continue;
                }

                var tasks = new List<IWorkerTask>();
                while (_taskQueue.TryDequeue(out var task))
                    tasks.Add(task);

                await RunTasksAsync(tasks, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error while working on task queue");
            }
            finally
            {
                _logger.LogInformation("Worker is sleeping until {WakeTime}", DateTime.Now.Add(sleepDuration));

                await Task.Delay(sleepDuration, stoppingToken);
            }
        }
    }

    private async Task RunTasksAsync(List<IWorkerTask> tasks, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Running {TaskCount} worker task(s)", tasks.Count);

        foreach (var workerTask in tasks)
        {
            await RunTaskAsync(workerTask, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
                continue;

            _logger.LogTrace("Cancellation requested. Aborting running tasks");

            return;
        }

        _logger.LogTrace("Done running tasks");
    }

    private async Task RunTaskAsync(IWorkerTask task, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (_logger.BeginScope(new Dictionary<string, string>
               {
                   { "TaskName", task.Name },
               }))
        {
            try
            {
                _logger.LogDebug("Running task {TaskName} (try #{Try})", task.Name, task.Tries);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = await task.Invoke(_serviceProvider, cancellationToken);
                stopwatch.Stop();

                HandleTaskResult(task, result, stopwatch);
            }
            catch (Exception e)
            {
                task.Tries++;

                _logger.LogError(e, "Error while running worker task");

                if (task is { Tries: > MaxTries, RequeueOnException: true })
                {
                    _logger.LogError("Not re-queueing task due to it having reached the maximum number of tries ({MaxTries})", MaxTries);
                }
                else if (task.RequeueOnException)
                {
                    _logger.LogTrace("Re-queueing failed task");

                    _taskQueue.Enqueue(task);
                }
                else
                {
                    _logger.LogTrace("Not re-queueing failed task");
                }
            }
        }
    }

    private void HandleTaskResult(IWorkerTask task, WorkerTaskResult result, Stopwatch stopwatch)
    {
        switch (result)
        {
            case WorkerTaskResult.Done:
            case WorkerTaskResult.Requeue:
                task.Tries = 0;

                _logger.LogDebug("Task {TaskName} ran successfully in {Elapsed}ms",
                    task.Name, stopwatch.Elapsed.TotalMilliseconds);

                if (result != WorkerTaskResult.Requeue)
                    return;

                _logger.LogTrace("Re-queuing task {TaskName}", task.Name);

                _taskQueue.Enqueue(task);

                return;
            case WorkerTaskResult.FailedRecoverable:
                task.Tries++;

                _logger.LogError("Task failed after {Elapsed}ms (recoverable). Re-queueing task",
                    stopwatch.Elapsed.TotalMilliseconds);

                if (task.Tries > MaxTries)
                {
                    _logger.LogError("Not re-queueing task due to it having reached the maximum number of tries ({MaxTries})", MaxTries);

                    return;
                }

                _taskQueue.Enqueue(task);
                return;
            case WorkerTaskResult.Failed:
                _logger.LogError("Task failed after {Elapsed}ms (fatal). Not re-queueing",
                    stopwatch.Elapsed.TotalMilliseconds);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, "Unimplemented worker result");
        }
    }
}