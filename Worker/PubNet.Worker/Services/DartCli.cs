using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PubNet.Worker.Services;

public class DartCli
{
	private readonly ILogger<DartCli> logger;
	private string? dartBinary;

	public DartCli(ILogger<DartCli> logger)
	{
		this.logger = logger;
	}

	private async Task<string> FindDartBinaryAsync(CancellationToken cancellationToken = default)
	{
		if (dartBinary is not null)
			return dartBinary;

		async Task<string> ConfigureRunProcess(string findBin, string dartBin)
		{
			var process = Process.Start(new ProcessStartInfo
			{
				FileName = findBin,
				Arguments = dartBin,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			});

			if (process is null) throw new($"Unable to search {dartBin}");

			await process.WaitForExitAsync(cancellationToken);

			var errors = await process.StandardError.ReadToEndAsync(cancellationToken);
			if (process.ExitCode != 0) throw new($"Unable to find {dartBin} ({errors})");

			if (errors.Length > 0) throw new(errors);

			var binPath = await process.StandardOutput.ReadLineAsync(cancellationToken);
			if (binPath is null) throw new("Unable to determine binary path");

			return binPath;
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			dartBinary = await ConfigureRunProcess("where.exe", "dart.bat");
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			dartBinary = await ConfigureRunProcess("which", "dart");
		else
			throw new NotSupportedException("Your platform is not supported.");

		Debug.Assert(dartBinary is not null, "dartBinary is not null");

		return dartBinary;
	}

	public async Task<int> Format(string folder, string workingDir, CancellationToken cancellationToken = default)
	{
		return await InvokeDart($"format {folder} --set-exit-if-changed", workingDir, cancellationToken);
	}

	public async Task<int> Doc(string workingDir, CancellationToken cancellationToken = default)
	{
		return await InvokeDart("doc", workingDir, cancellationToken);
	}

	[Obsolete("Rewrite usage of this class to use CliWrap instead (https://github.com/ricardoboss/PubNet/issues/6)")]
	public async Task<int> InvokeDart(string command, string workingDirectory,
		CancellationToken cancellationToken = default)
	{
		var psi = new ProcessStartInfo
		{
			FileName = await FindDartBinaryAsync(cancellationToken),
			Arguments = command,
			WorkingDirectory = workingDirectory,
			UseShellExecute = true,
		};

		logger.LogTrace("Invoking dart binary with {Command} in {WorkingDirectory}", command, workingDirectory);

		using var proc = Process.Start(psi);
		if (proc is null) throw new("Unable to start dart process");

		await proc.WaitForExitAsync(cancellationToken);

		return proc.ExitCode;
	}
}
