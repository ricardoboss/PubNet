namespace PubNet.API.Services;

public interface IWorkerTask
{
    Task<WorkerTaskResult> Invoke(IServiceProvider services, CancellationToken cancellationToken = default);

    string Name { get; }

    bool RequeueOnException { get; }

    DateTimeOffset NextRun { get; set; }

    int Tries { get; set; }
}
