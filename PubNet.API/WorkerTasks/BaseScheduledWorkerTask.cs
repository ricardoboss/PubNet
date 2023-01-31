using PubNet.API.Services;

namespace PubNet.API.WorkerTasks;

public abstract class BaseScheduledWorkerTask : BaseWorkerTask, IScheduledWorkerTask
{
    /// <inheritdoc />
    public TimeSpan Interval { get; }

    /// <inheritdoc />
    public DateTime ScheduledAt { get; }

    /// <inheritdoc />
    public DateTime? LastRun { get; protected set; }

    /// <inheritdoc />
    public DateTime NextRun { get; protected set; }

    protected BaseScheduledWorkerTask(TimeSpan interval, DateTime scheduledAt, string? name = null, bool? requeueOnException = null) : base(name, requeueOnException)
    {
        Interval = interval;
        ScheduledAt = scheduledAt;
        LastRun = null;
        NextRun = scheduledAt.Add(interval);
    }

    /// <inheritdoc />
    protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        try
        {
            return await InvokeScheduled(services, cancellationToken);
        }
        finally
        {
            LastRun = DateTime.Now;
            NextRun = DateTime.Now.Add(Interval);
        }
    }

    protected abstract Task<WorkerTaskResult> InvokeScheduled(IServiceProvider services, CancellationToken cancellationToken = default);
}
