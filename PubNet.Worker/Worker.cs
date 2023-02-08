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

	private TimeSpan GetInterval(string path)
	{
		return TimeSpan.Parse(_configuration.GetRequiredSection(path).Value ?? string.Empty);
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		_taskQueue.Enqueue(new CleanupOldPendingArchivesTask(GetInterval("Worker:PendingCleanupInterval")));
		_taskQueue.Enqueue(new MissingPackageVersionAnalysisQueuingTask(GetInterval("Worker:QueueMissingAnalysisInterval")));

		await base.StartAsync(cancellationToken);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var interval = GetInterval("Worker:TaskInterval");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				_logger.LogInformation("Worker is running at {Now}", DateTime.Now);

				await RunScheduledTasksAsync(DateTime.Now, stoppingToken);

				// adjust wake time after running scheduled tasks
				var wakeTime = AdjustWakeTime(null, interval, out var runNow);
				if (runNow)
				{
					_logger.LogInformation("Skipping unscheduled tasks and sleep as scheduled tasks are due now");

					continue;
				}

				await RunUnscheduledTasks(stoppingToken);

				var sleepDuration  = wakeTime.Subtract(DateTime.Now);
				if (sleepDuration <= TimeSpan.Zero)
				{
					_logger.LogInformation("Skipping sleep as wake time has already passed ({WakeTime})", wakeTime);

					continue;
				}

				if (_taskQueue.GetNextUnscheduled(out var task))
				{
					_logger.LogInformation("Skipping sleep because new, unscheduled tasks have been queued ({TaskName})", task.Name);

					continue;
				}

				_logger.LogInformation("Worker is sleeping until {WakeTime}", wakeTime);

				await Task.Delay(sleepDuration, _taskQueue.SleepCancellation);
			}
			catch (Exception e)
			{
				_logger.LogCritical(e, "Error while working on task queue");

				break;
			}
		}

		_logger.LogWarning("Worker is stopping");
	}

	private DateTime AdjustWakeTime(DateTime? wakeTime, TimeSpan interval, out bool runNow)
	{
		runNow = false;
		var calculatedWakeTime = wakeTime ?? DateTime.Now.Add(interval);
		if (!_taskQueue.TryGetNextScheduledAt(out var scheduledTask, out var nextScheduledTime))
		{
			_logger.LogDebug("No scheduled tasks found");

			return calculatedWakeTime;
		}

		if (nextScheduledTime <= DateTime.Now)
		{
			_logger.LogDebug("Scheduled tasks are due now ({TaskName})", scheduledTask.Name);

			runNow = true;
			return nextScheduledTime;
		}

		if (nextScheduledTime >= calculatedWakeTime)
			return calculatedWakeTime; // worker is awake at or before next scheduled task

		_logger.LogDebug(
			"Worker wake time adjusted to {NextScheduled} (because of {TaskName})",
			nextScheduledTime,
			scheduledTask.Name
		);

		return nextScheduledTime;
	}

	private async Task RunScheduledTasksAsync(DateTime limit, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var count = 0;
		foreach (var workerTask in _taskQueue.DequeueUntil(limit))
		{
			await RunTaskAsync(workerTask, cancellationToken);

			count++;

			if (!cancellationToken.IsCancellationRequested)
				continue;

			_logger.LogInformation("Cancellation requested. Aborting running tasks");

			return;
		}

		_logger.LogInformation("Ran {Count} scheduled tasks up until {Limit}", count, limit);
	}

	private async Task RunUnscheduledTasks(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var count = 0;
		while (_taskQueue.DequeueUnscheduled(out var task))
		{
			await RunTaskAsync(task, cancellationToken);

			count++;

			if (!cancellationToken.IsCancellationRequested) continue;

			_logger.LogInformation("Cancellation requested. Aborting running tasks");

			break;
		}

		_logger.LogInformation("Ran {Count} unscheduled tasks", count);
	}

	private async Task RunTaskAsync(IWorkerTask task, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		using (_logger.BeginScope(new Dictionary<string, string?>
		{
			{ "TaskName", task.Name },
			{ "TaskType", task.GetType().FullName },
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
					_logger.LogDebug("Re-queueing failed task");

					_taskQueue.Enqueue(task);
				}
				else
				{
					_logger.LogDebug("Not re-queueing failed task");
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
