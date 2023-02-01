namespace PubNet.Worker.Models;

public enum WorkerTaskResult
{
	Done,
	Requeue,
	FailedRecoverable,
	Failed,
}

public static class WorkerTaskResultExtensions
{
	public static bool IsSuccess(this WorkerTaskResult result)
	{
		switch (result)
		{
			case WorkerTaskResult.Done:
			case WorkerTaskResult.Requeue:
				return true;
			case WorkerTaskResult.FailedRecoverable:
			case WorkerTaskResult.Failed:
			default:
				return false;
		}
	}

	public static bool IndicatesRequeue(this WorkerTaskResult result)
	{
		switch (result)
		{
			case WorkerTaskResult.Requeue:
			case WorkerTaskResult.FailedRecoverable:
				return true;
			case WorkerTaskResult.Done:
			case WorkerTaskResult.Failed:
			default:
				return false;
		}
	}
}
