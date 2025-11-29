using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using PubNet.Worker.Models;

namespace PubNet.Worker.Services;

public class WorkerTaskQueue : IDisposable
{
	private readonly ILogger<WorkerTaskQueue> _logger;
	private readonly ConcurrentQueue<IWorkerTask> _queue = new();
	private readonly PriorityQueue<IScheduledWorkerTask, DateTime> _scheduledQueue = new();
	private CancellationTokenSource _sleepCancellation = new();

	public CancellationToken SleepCancellation => _sleepCancellation.Token;

	public WorkerTaskQueue(ILogger<WorkerTaskQueue> logger)
	{
		_logger = logger;
	}

	public void Enqueue(IWorkerTask item)
	{
		if (item is IScheduledWorkerTask scheduled)
		{
			_scheduledQueue.Enqueue(scheduled, scheduled.NextRun);

			_logger.LogTrace("Scheduled task queued: {TaskName} (due at {NextScheduled})", item.Name, scheduled.NextRun);
		}
		else
		{
			_queue.Enqueue(item);

			_logger.LogTrace("Task queued: {TaskName}", item.Name);

			_sleepCancellation.Cancel();
			if (_sleepCancellation.TryReset()) return;

			_sleepCancellation.Dispose();
			_sleepCancellation = new();
		}
	}

	public bool GetNextUnscheduled([NotNullWhen(true)] out IWorkerTask? task)
	{
		return _queue.TryPeek(out task);
	}

	public bool DequeueUnscheduled([NotNullWhen(true)] out IWorkerTask? task)
	{
		return _queue.TryDequeue(out task);
	}

	public bool TryGetNextScheduledAt([NotNullWhen(true)] out IScheduledWorkerTask? task, out DateTime scheduledAt)
	{
		return _scheduledQueue.TryPeek(out task, out scheduledAt);
	}

	public IEnumerable<IScheduledWorkerTask> DequeueUntil(DateTime limit)
	{
		while (_scheduledQueue.TryDequeue(out var task, out var scheduledAt))
		{
			if (scheduledAt <= limit)
				yield return task;
			else
			{
				// re-queue task that is above limit and break out of the loop
				_scheduledQueue.Enqueue(task, scheduledAt);

				break;
			}
		}
	}

	public IEnumerable<IWorkerTask> Tasks => _queue.Union(_scheduledQueue.UnorderedItems.Select(t => t.Element));

	/// <inheritdoc />
	public void Dispose()
	{
		_sleepCancellation.Dispose();
	}
}
