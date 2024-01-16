using System.Runtime.CompilerServices;

namespace PubNet.Frontend.Services;

public class FetchLock<T>
{
	private const int FetchDelay = 50;

	private readonly ILogger<FetchLock<T>> _logger;
	private readonly string _classPrefix;

	private bool _locked;
	private string _lockedBy = "another task";

	public FetchLock(ILogger<FetchLock<T>> logger)
	{
		_logger = logger;
		_classPrefix = typeof(T).Name;
	}

	public async Task UntilFreed(int delay = FetchDelay, [CallerMemberName] string taskName = "")
	{
		while (_locked)
		{
			_logger.LogTrace("[{ClassPrefix}] {TaskName} is waiting until freed by {LockedBy}", _classPrefix, taskName, _lockedBy);

			await Task.Delay(delay);
		}

		_logger.LogTrace("[{ClassPrefix}] {TaskName} can now acquire the lock", _classPrefix, taskName);
	}

	public void Lock([CallerMemberName] string taskName = "")
	{
		if (_locked)
			throw new InvalidOperationException("Already locked");

		_locked = true;
		_lockedBy = taskName;

		_logger.LogTrace("[{ClassPrefix}] {TaskName} has acquired the lock", _classPrefix, taskName);
	}

	public async Task LockAfterFreed(int delay = FetchDelay, [CallerMemberName] string taskName = "")
	{
		await UntilFreed(delay, taskName);

		Lock(taskName);
	}

	public void Free()
	{
		_locked = false;

		_logger.LogTrace("[{ClassPrefix}] {TaskName} has freed the lock", _classPrefix, _lockedBy);
	}
}
