using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public abstract class BaseScheduledWorkerTask : BaseWorkerTask, IScheduledWorkerTask
{
	protected BaseScheduledWorkerTask(TimeSpan interval, DateTime scheduledAt, string? name = null,
		bool? requeueOnException = null) : base(name, requeueOnException)
	{
		Interval = interval;
		ScheduledAt = scheduledAt;
		LastRun = null;
		NextRun = scheduledAt;
	}

	/// <inheritdoc />
	public TimeSpan Interval { get; protected set; }

	/// <inheritdoc />
	public DateTime ScheduledAt { get; }

	/// <inheritdoc />
	public DateTime? LastRun { get; private set; }

	/// <inheritdoc />
	public DateTime NextRun { get; private set; }

	/// <inheritdoc />
	protected override async Task<WorkerTaskResult> InvokeInternal(IServiceProvider services,
		CancellationToken cancellationToken = default)
	{
		try
		{
			LastRun = DateTime.Now;

			return await InvokeScheduled(services, cancellationToken);
		}
		finally
		{
			NextRun = DateTime.Now.Add(Interval);
		}
	}

	protected abstract Task<WorkerTaskResult> InvokeScheduled(IServiceProvider services,
		CancellationToken cancellationToken = default);
}
