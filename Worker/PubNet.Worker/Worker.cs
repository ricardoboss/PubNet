using System.Diagnostics;
using PubNet.Worker.Models;
using PubNet.Worker.Services;
using PubNet.Worker.Tasks;

namespace PubNet.Worker;

public class Worker : BackgroundService
{
	private const int MaxTries = 10;
	private readonly IConfiguration configuration;

	private readonly ILogger<Worker> logger;
	private readonly IServiceProvider serviceProvider;
	private readonly WorkerTaskQueue taskQueue;

	public Worker(ILogger<Worker> logger, WorkerTaskQueue taskQueue, IServiceProvider serviceProvider,
		IConfiguration configuration)
	{
		this.logger = logger;
		this.taskQueue = taskQueue;
		this.serviceProvider = serviceProvider;
		this.configuration = configuration;
	}

	private TimeSpan GetInterval(string path)
	{
		return TimeSpan.Parse(configuration.GetRequiredSection(path).Value ?? string.Empty);
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		taskQueue.Enqueue(new CleanupOldPendingArchivesTask(GetInterval("Worker:PendingCleanupInterval")));
		taskQueue.Enqueue(new MissingPackageVersionAnalysisQueuingTask(GetInterval("Worker:QueueMissingAnalysisInterval")));
		taskQueue.Enqueue(new FlutterUpgradeTask(GetInterval("Worker:FlutterUpgradeInterval")));

		await base.StartAsync(cancellationToken);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var interval = GetInterval("Worker:TaskInterval");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				logger.LogInformation("Worker is running at {Now}", DateTime.Now);

				await RunScheduledTasksAsync(DateTime.Now, stoppingToken);

				// calculate wake time after running scheduled tasks
				var wakeTime = CalculateWakeTime(interval, out var runNow);
				if (runNow)
				{
					logger.LogInformation("Skipping unscheduled tasks and sleep as scheduled tasks are due now");

					continue;
				}

				await RunUnscheduledTasks(stoppingToken);

				var sleepDuration = wakeTime.Subtract(DateTime.Now);
				if (sleepDuration <= TimeSpan.Zero)
				{
					logger.LogInformation("Skipping sleep as wake time has already passed ({WakeTime})", wakeTime);

					continue;
				}

				if (taskQueue.GetNextUnscheduled(out var task))
				{
					logger.LogInformation("Skipping sleep because new, unscheduled tasks have been queued ({TaskName})", task.Name);

					continue;
				}

				logger.LogInformation("Worker is sleeping until {WakeTime}", wakeTime);

				await Task.Delay(sleepDuration, taskQueue.SleepCancellation);
			}
			catch (Exception e)
			{
				logger.LogCritical(e, "Error while working on task queue");

				break;
			}
		}

		logger.LogWarning("Worker is stopping");
	}

	private DateTime CalculateWakeTime(TimeSpan interval, out bool runNow)
	{
		runNow = false;
		var calculatedWakeTime = DateTime.Now.Add(interval);
		if (!taskQueue.TryGetNextScheduledAt(out var scheduledTask, out var nextScheduledTime))
		{
			logger.LogDebug("No scheduled tasks found");

			return calculatedWakeTime;
		}

		if (nextScheduledTime <= DateTime.Now)
		{
			logger.LogDebug("Scheduled tasks are due now ({TaskName})", scheduledTask.Name);

			runNow = true;
			return nextScheduledTime;
		}

		if (nextScheduledTime >= calculatedWakeTime)
			return calculatedWakeTime; // worker is awake at or before next scheduled task

		logger.LogDebug(
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
		foreach (var workerTask in taskQueue.DequeueUntil(limit))
		{
			await RunTaskAsync(workerTask, cancellationToken);

			count++;

			if (!cancellationToken.IsCancellationRequested)
				continue;

			logger.LogInformation("Cancellation requested. Aborting running tasks");

			return;
		}

		logger.LogInformation("Ran {Count} scheduled tasks up until {Limit}", count, limit);
	}

	private async Task RunUnscheduledTasks(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var count = 0;
		while (taskQueue.DequeueUnscheduled(out var task))
		{
			await RunTaskAsync(task, cancellationToken);

			count++;

			if (!cancellationToken.IsCancellationRequested) continue;

			logger.LogInformation("Cancellation requested. Aborting running tasks");

			break;
		}

		logger.LogInformation("Ran {Count} unscheduled tasks", count);
	}

	private async Task RunTaskAsync(IWorkerTask task, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		using (logger.BeginScope(new Dictionary<string, string?>
		{
			{ "TaskName", task.Name },
			{ "TaskType", task.GetType().FullName },
		}))
		{
			try
			{
				logger.LogDebug("Running task {TaskName} (try #{Try})", task.Name, task.Tries);

				var stopwatch = new Stopwatch();
				stopwatch.Start();
				var result = await task.Invoke(serviceProvider, cancellationToken);
				stopwatch.Stop();

				HandleTaskResult(task, result, stopwatch);
			}
			catch (Exception e)
			{
				logger.LogError(e, "Error while running worker task");

				if (task is { Tries: >= MaxTries, RequeueOnException: true })
				{
					logger.LogError(
						"Not re-queueing task due to it having reached the maximum number of tries ({MaxTries})",
						MaxTries);
				}
				else if (task.RequeueOnException)
				{
					logger.LogDebug("Re-queueing failed task");

					taskQueue.Enqueue(task);
				}
				else
				{
					logger.LogDebug("Not re-queueing failed task");
				}
			}
		}
	}

	private void HandleTaskResult(IWorkerTask task, WorkerTaskResult result, Stopwatch stopwatch)
	{
		if (result.IsSuccess())
			logger.LogDebug("Task {TaskName} succeeded in {Elapsed}ms", task.Name,
				stopwatch.Elapsed.TotalMilliseconds);
		else
			logger.LogError("Task {TaskName} failed after {Elapsed}ms", task.Name,
				stopwatch.Elapsed.TotalMilliseconds);

		if (!result.IndicatesRequeue()) return;

		if (task.Tries >= MaxTries)
		{
			logger.LogError(
				"Not re-queueing task due to it having reached the maximum number of tries ({MaxTries})", MaxTries);

			return;
		}

		taskQueue.Enqueue(task);
	}
}
