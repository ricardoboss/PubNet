using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace PubNet.SDK.Helpers;

public class FetchLock<T>(ILogger<FetchLock<T>> logger)
{
	private const int FetchDelay = 50;

	private readonly string _classPrefix = typeof(T).Name;

	private bool _locked;
	private string _lockedBy = "another task";

	public async Task UntilFreed(int delay = FetchDelay, [CallerMemberName] string taskName = "")
	{
		while (_locked)
		{
			logger.LogTrace("[{ClassPrefix}] {TaskName} is waiting until freed by {LockedBy}", _classPrefix, taskName, _lockedBy);

			await Task.Delay(delay);
		}

		logger.LogTrace("[{ClassPrefix}] {TaskName} can now acquire the lock", _classPrefix, taskName);
	}

	[MustDisposeResource]
	public IDisposable Lock([CallerMemberName] string taskName = "")
	{
		if (_locked)
			throw new InvalidOperationException("Already locked");

		_locked = true;
		_lockedBy = taskName;

		logger.LogTrace("[{ClassPrefix}] {TaskName} has acquired the lock", _classPrefix, taskName);

		return new FetchLockObj(Free);
	}

	[MustDisposeResource]
	public async Task<IDisposable> UntilFreedAndLock(int delay = FetchDelay, [CallerMemberName] string taskName = "")
	{
		await UntilFreed(delay, taskName);

		return Lock(taskName);
	}

	private void Free()
	{
		_locked = false;

		logger.LogTrace("[{ClassPrefix}] {TaskName} has freed the lock", _classPrefix, _lockedBy);
	}
}

file class FetchLockObj(Action free) : IDisposable
{
	public void Dispose() => free();
}
