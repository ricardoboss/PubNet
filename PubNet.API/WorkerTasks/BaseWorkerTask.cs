using PubNet.API.Services;

namespace PubNet.API.WorkerTasks;

public abstract class BaseWorkerTask : IWorkerTask
{
    public abstract Task<WorkerTaskResult> Invoke(IServiceProvider services, CancellationToken cancellationToken = default);

    public virtual string Name => GetType().Name;

    public abstract bool RequeueOnException { get; }

    public virtual DateTimeOffset NextRun { get; set; } = DateTimeOffset.Now;

    public virtual int Tries { get; set; } = 0;
}
