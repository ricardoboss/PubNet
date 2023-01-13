using System.Collections.Concurrent;

namespace PubNet.API.Services;

public class WorkerTaskQueue : ConcurrentQueue<IWorkerTask>
{
    private readonly ILogger<WorkerTaskQueue> _logger;

    public WorkerTaskQueue(ILogger<WorkerTaskQueue> logger)
    {
        _logger = logger;
    }

    public new void Enqueue(IWorkerTask item)
    {
        base.Enqueue(item);

        _logger.LogInformation("New task queued for worker queue: {TaskName}", item.Name);
    }
}