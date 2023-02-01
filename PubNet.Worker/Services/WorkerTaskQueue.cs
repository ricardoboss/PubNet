using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using PubNet.Worker.Models;

namespace PubNet.Worker.Services;

public class WorkerTaskQueue
{
	private readonly ILogger<WorkerTaskQueue> _logger;
	private readonly ConcurrentQueue<IWorkerTask> _queue = new();
	private readonly PriorityQueue<IScheduledWorkerTask, DateTime> _scheduledQueue = new();

	public WorkerTaskQueue(ILogger<WorkerTaskQueue> logger)
	{
		_logger = logger;
	}

	public void Enqueue(IWorkerTask item)
	{
		if (item is IScheduledWorkerTask scheduled)
			_scheduledQueue.Enqueue(scheduled, scheduled.NextRun);
		else
			_queue.Enqueue(item);

		_logger.LogDebug("New task queued for worker queue: {TaskName}", item.Name);
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
		while (_scheduledQueue.TryDequeue(out var task, out var scheduledAt) && scheduledAt <= limit) yield return task;
	}
}
