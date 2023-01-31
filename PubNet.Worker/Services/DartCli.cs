using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PubNet.Worker.Services;

public class DartCli
{
    private readonly ILogger<DartCli> _logger;
    private string? _dartBinary;

    public DartCli(ILogger<DartCli> logger)
    {
        _logger = logger;
    }

    private async Task<string> FindDartBinaryAsync(CancellationToken cancellationToken = default)
    {
        if (_dartBinary is not null)
            return _dartBinary;

        async Task<string> RunProcess(string findBin, string dartBin)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = findBin,
                Arguments = dartBin,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            });

            if (process is null)
            {
                throw new($"Unable to search {dartBin}");
            }

            await process.WaitForExitAsync(cancellationToken);

            var errors = await process.StandardError.ReadToEndAsync(cancellationToken);
            if (process.ExitCode != 0)
            {
                throw new($"Unable to find {dartBin} ({errors})");
            }

            if (errors.Length > 0)
            {
                throw new(errors);
            }

            var binPath = await process.StandardOutput.ReadLineAsync(cancellationToken);
            if (binPath is null)
            {
                throw new("Unable to determine binary path");
            }

            return binPath;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _dartBinary = await RunProcess("where.exe", "dart.bat");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _dartBinary = await RunProcess("which", "dart");
        }
        else
        {
            throw new NotSupportedException("Your platform is not supported.");
        }

        Debug.Assert(_dartBinary is not null, "_dartBinary is not null");

        return _dartBinary;
    }

    public async Task<int> Format(string folder, string workingDir, CancellationToken cancellationToken = default)
    {
        return await InvokeDart($"format {folder} --set-exit-if-changed", workingDir, cancellationToken);
    }

    public async Task<int> Doc(string workingDir, CancellationToken cancellationToken = default)
    {
        return await InvokeDart("doc", workingDir, cancellationToken);
    }

    private async Task<int> InvokeDart(string command, string workingDirectory,
        CancellationToken cancellationToken = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName = await FindDartBinaryAsync(cancellationToken),
            Arguments = command,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        var proc = Process.Start(psi);
        if (proc is null)
        {
            throw new("Unable to start dart process");
        }

        proc.OutputDataReceived += (_, args) =>
        {
            if (args.Data != null) _logger.LogTrace("{DartCliOutput}", args.Data);
        };

        proc.ErrorDataReceived += (_, args) =>
        {
            if (args.Data != null) _logger.LogTrace("{DartCliError}", args.Data);
        };

        await proc.WaitForExitAsync(cancellationToken);

        return proc.ExitCode;
    }
}