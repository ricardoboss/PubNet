using PubNet.API.Services;

namespace PubNet.API.WorkerTasks;

public abstract class BaseWorkerTask : IWorkerTask
{
    public virtual string Name { get; }

    public virtual bool RequeueOnException { get; }

    public virtual int Tries { get; private set; }

    protected BaseWorkerTask(string? name = null, bool? requeueOnException = null)
    {
        Name = name ?? GetType().Name;
        RequeueOnException = requeueOnException ?? true;
    }

    public virtual async Task<WorkerTaskResult> Invoke(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await InvokeInternal(services, cancellationToken);

            Tries = 0;

            return result;
        }
        catch (Exception)
        {
            Tries++;

            throw;
        }
    }

    protected abstract Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default);
}
