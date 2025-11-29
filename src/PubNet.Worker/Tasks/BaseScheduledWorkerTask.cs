using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public abstract class BaseScheduledWorkerTask(
	TimeSpan interval,
	DateTime scheduledAt,
	string? name = null,
	bool? requeueOnException = null
) : BaseWorkerTask(name, requeueOnException), IScheduledWorkerTask
{
	/// <inheritdoc />
	public TimeSpan Interval { get; protected set; } = interval;

	/// <inheritdoc />
	public DateTime ScheduledAt { get; } = scheduledAt;

	/// <inheritdoc />
	public DateTime? LastRun { get; private set; }

	/// <inheritdoc />
	public DateTime NextRun { get; private set; } = scheduledAt;

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
