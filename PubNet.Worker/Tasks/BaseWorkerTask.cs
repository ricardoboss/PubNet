using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public abstract class BaseWorkerTask : IWorkerTask
{
	protected BaseWorkerTask(string? name = null, bool? requeueOnException = null)
	{
		Name = name ?? GetType().Name;
		RequeueOnException = requeueOnException ?? true;
	}

	public virtual string Name { get; }

	public virtual bool RequeueOnException { get; }

	public virtual int Tries { get; private set; }

	public virtual async Task<WorkerTaskResult> Invoke(IServiceProvider services,
		CancellationToken cancellationToken = default)
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

	protected abstract Task<WorkerTaskResult> InvokeInternal(IServiceProvider services,
		CancellationToken cancellationToken = default);
}
