using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using PubNet.Worker.Models;

namespace PubNet.Worker.Services;

public class WorkerTaskQueue : IDisposable
{
	private readonly ILogger<WorkerTaskQueue> logger;
	private readonly ConcurrentQueue<IWorkerTask> queue = new();
	private readonly PriorityQueue<IScheduledWorkerTask, DateTime> scheduledQueue = new();
	private CancellationTokenSource sleepCancellation = new();

	public CancellationToken SleepCancellation => sleepCancellation.Token;

	public WorkerTaskQueue(ILogger<WorkerTaskQueue> logger)
	{
		this.logger = logger;
	}

	public void Enqueue(IWorkerTask item)
	{
		if (item is IScheduledWorkerTask scheduled)
		{
			scheduledQueue.Enqueue(scheduled, scheduled.NextRun);

			logger.LogTrace("Scheduled task queued: {TaskName} (due at {NextScheduled})", item.Name, scheduled.NextRun);
		}
		else
		{
			queue.Enqueue(item);

			logger.LogTrace("Task queued: {TaskName}", item.Name);

			sleepCancellation.Cancel();
			if (sleepCancellation.TryReset()) return;

			sleepCancellation.Dispose();
			sleepCancellation = new();
		}
	}

	public bool GetNextUnscheduled([NotNullWhen(true)] out IWorkerTask? task)
	{
		return queue.TryPeek(out task);
	}

	public bool DequeueUnscheduled([NotNullWhen(true)] out IWorkerTask? task)
	{
		return queue.TryDequeue(out task);
	}

	public bool TryGetNextScheduledAt([NotNullWhen(true)] out IScheduledWorkerTask? task, out DateTime scheduledAt)
	{
		return scheduledQueue.TryPeek(out task, out scheduledAt);
	}

	public IEnumerable<IScheduledWorkerTask> DequeueUntil(DateTime limit)
	{
		while (scheduledQueue.TryDequeue(out var task, out var scheduledAt))
		{
			if (scheduledAt <= limit)
				yield return task;
			else
			{
				// re-queue task that is above limit and break out of the loop
				scheduledQueue.Enqueue(task, scheduledAt);

				break;
			}
		}
	}

	public IEnumerable<IWorkerTask> Tasks => queue.Union(scheduledQueue.UnorderedItems.Select(t => t.Element));

	/// <inheritdoc />
	public void Dispose()
	{
		sleepCancellation.Dispose();
	}
}
