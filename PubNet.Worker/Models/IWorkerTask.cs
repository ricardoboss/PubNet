namespace PubNet.Worker.Models;

public interface IWorkerTask
{
    string Name { get; }

    bool RequeueOnException { get; }

    int Tries { get; }

    Task<WorkerTaskResult> Invoke(IServiceProvider services, CancellationToken cancellationToken = default);
}
