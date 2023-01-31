namespace PubNet.API.Services;

public interface IScheduledWorkerTask : IWorkerTask
{
    TimeSpan Interval { get; }

    DateTime ScheduledAt { get; }

    DateTime? LastRun { get; }

    DateTime NextRun { get; }
}
