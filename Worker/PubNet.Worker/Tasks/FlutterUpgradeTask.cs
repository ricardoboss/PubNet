using CliWrap;
using PubNet.Worker.Models;

namespace PubNet.Worker.Tasks;

public class FlutterUpgradeTask : BaseScheduledWorkerTask
{
	private static readonly TimeSpan MaxBackOffInterval = TimeSpan.FromDays(3);

	private ILogger<FlutterUpgradeTask>? maybeLogger;

	/// <inheritdoc />
	public FlutterUpgradeTask(TimeSpan interval) : base(interval, DateTime.Now, nameof(FlutterUpgradeTask), true)
	{
	}

	/// <inheritdoc />
	protected override async Task<WorkerTaskResult> InvokeScheduled(IServiceProvider services, CancellationToken cancellationToken = default)
	{
		maybeLogger ??= services.GetRequiredService<ILogger<FlutterUpgradeTask>>();

		var upgradeResult = await RunFlutterCommand(maybeLogger, "upgrade", cancellationToken);
		if (!upgradeResult.IsSuccess())
			return upgradeResult;

		return await RunFlutterCommand(maybeLogger, "precache", cancellationToken);
	}

	private async Task<WorkerTaskResult> RunFlutterCommand(ILogger logger, string command, CancellationToken cancellationToken = default)
	{
		var result = await Cli.Wrap("flutter")
			.WithArguments(command)
			.WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
			.WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
			.ExecuteAsync(cancellationToken);

		if (result.ExitCode == 0) return WorkerTaskResult.Requeue;

		// exponential back-off
		Interval *= 2;
		if (Interval >= MaxBackOffInterval)
		{
			logger.LogError("Flutter {Command} failed too many times. Restart the worker to enable this task again", command);

			return WorkerTaskResult.Failed;
		}

		logger.LogError("Flutter {Command} failed. Will try again in {Interval}", command, Interval);

		return WorkerTaskResult.FailedRecoverable;
	}
}
