using System.Diagnostics;
using PubNet.Worker.Models;
using PubNet.Worker.Services;
using PubNet.Worker.Tasks;

namespace PubNet.Worker;

public class Worker : BackgroundService
{
	private const int MaxTries = 10;
	private readonly IConfiguration _configuration;

	private readonly ILogger<Worker> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly WorkerTaskQueue _taskQueue;

	public Worker(ILogger<Worker> logger, WorkerTaskQueue taskQueue, IServiceProvider serviceProvider,
		IConfiguration configuration)
	{
		_logger = logger;
		_taskQueue = taskQueue;
		_serviceProvider = serviceProvider;
		_configuration = configuration;
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		_taskQueue.Enqueue(new CleanupOldPendingArchivesTask(TimeSpan.FromMinutes(1)));
		_taskQueue.Enqueue(new MissingAnalysisQueuingTask(TimeSpan.FromSeconds(35)));

		await base.StartAsync(cancellationToken);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (!TimeSpan.TryParse(_configuration.GetRequiredSection("Worker:Interval").Value!, out var interval))
		{
			_logger.LogCritical("Failed to parse Worker:SleepInterval value. Check your configuration");

			return;
		}

		while (!stoppingToken.IsCancellationRequested)
		{
			var start = DateTime.Now;
			DateTime? wakeTime = null;
			try
			{
				var tasks = new List<IWorkerTask>();
				while (_taskQueue.DequeueUnscheduled(out var task))
					tasks.Add(task);

				if (_taskQueue.TryGetNextScheduledAt(out var scheduledTask, out var scheduledAt))
				{
					// if the next scheduled task should run in the future, abort
					if (scheduledAt > DateTimeOffset.Now)
					{
						_logger.LogTrace("Next scheduled task should run at {NextScheduled}", scheduledAt);

						if (scheduledAt < (wakeTime ?? DateTime.Now.Add(interval)))
						{
							_logger.LogInformation("Adjusting worker wake time for scheduled task {TaskName}",
								scheduledTask.Name);

							wakeTime = scheduledAt;
						}
						else
						{
							_logger.LogTrace("Worker wake time is before next scheduled task");
						}
					}
					else
					{
						tasks.AddRange(_taskQueue.DequeueUntil(scheduledAt));
					}
				}

				if (!tasks.Any())
				{
					_logger.LogInformation("No worker tasks to run right now");

					continue;
				}

				await RunTasksAsync(tasks, stoppingToken);

				if (_taskQueue.TryGetNextScheduledAt(out scheduledTask, out scheduledAt))
				{
					_logger.LogTrace("Next scheduled task should run at {NextScheduled}", scheduledAt);

					if (scheduledAt < (wakeTime ?? DateTime.Now.Add(interval)))
					{
						_logger.LogInformation("Adjusting worker wake time for scheduled task {TaskName}",
							scheduledTask.Name);

						wakeTime = scheduledAt;
					}
					else
					{
						_logger.LogTrace("Worker wake time is before next scheduled task");
					}
				}
			}
			catch (Exception e)
			{
				_logger.LogCritical(e, "Error while working on task queue");

				break;
			}
			finally
			{
				TimeSpan sleepDuration;
				if (wakeTime is null)
				{
					var runDuration = DateTime.Now - start;
					sleepDuration = interval - runDuration;
					wakeTime = DateTime.Now.Add(sleepDuration);
				}
				else
				{
					sleepDuration = wakeTime.Value.Subtract(DateTime.Now);
				}

				if (sleepDuration > TimeSpan.Zero)
				{
					_logger.LogInformation("Worker is sleeping until {WakeTime}", wakeTime);

					await Task.Delay(sleepDuration, stoppingToken);
				}
			}
		}

		_logger.LogWarning("Worker is stopping");
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
				_logger.LogError(e, "Error while running worker task");

				if (task is { Tries: >= MaxTries, RequeueOnException: true })
				{
					_logger.LogError(
						"Not re-queueing task due to it having reached the maximum number of tries ({MaxTries})",
						MaxTries);
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
		if (result.IsSuccess())
			_logger.LogDebug("Task {TaskName} succeeded in {Elapsed}ms", task.Name,
				stopwatch.Elapsed.TotalMilliseconds);
		else
			_logger.LogError("Task {TaskName} failed after {Elapsed}ms", task.Name,
				stopwatch.Elapsed.TotalMilliseconds);

		if (!result.IndicatesRequeue()) return;

		if (task.Tries >= MaxTries)
		{
			_logger.LogError(
				"Not re-queueing task due to it having reached the maximum number of tries ({MaxTries})", MaxTries);

			return;
		}

		_taskQueue.Enqueue(task);
	}
}
