namespace PubNet.API.Services;

public enum WorkerTaskResult
{
    Done,
    Requeue,
    FailedRecoverable,
    Failed,
}