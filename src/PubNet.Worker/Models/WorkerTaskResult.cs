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
	extension(WorkerTaskResult result)
	{
		public bool IsSuccess()
		{
			return result switch
			{
				WorkerTaskResult.Done or WorkerTaskResult.Requeue => true,
				_ => false,
			};
		}

		public bool IndicatesRequeue()
		{
			return result switch
			{
				WorkerTaskResult.Requeue or WorkerTaskResult.FailedRecoverable => true,
				_ => false,
			};
		}
	}
}
