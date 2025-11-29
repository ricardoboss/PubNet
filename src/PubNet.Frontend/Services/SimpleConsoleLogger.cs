using Microsoft.JSInterop;

namespace PubNet.Frontend.Services;

public class SimpleConsoleLoggerProvider(IJSRuntime jsRuntime) : ILoggerProvider
{
	/// <inheritdoc />
	public void Dispose()
	{
	}

	/// <inheritdoc />
	public ILogger CreateLogger(string categoryName)
	{
		return new SimpleConsoleLogger(categoryName, jsRuntime);
	}
}

public class SimpleConsoleLogger(string name, IJSRuntime jsRuntime) : ILogger
{
	/// <inheritdoc />
	public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		try
		{
			if (!IsEnabled(logLevel) || logLevel == LogLevel.None) return;

			await jsRuntime.InvokeVoidAsync(
				logLevel switch
				{
					LogLevel.Trace => "console.debug",
					LogLevel.Debug => "console.debug",
					LogLevel.Information => "console.log",
					LogLevel.Warning => "console.warn",
					LogLevel.Error => "console.error",
					LogLevel.Critical => "console.error",
					LogLevel.None => throw new InvalidOperationException(),
					_ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
				},
				$"[{name}] {formatter(state, exception)}"
			);
		}
		catch (Exception)
		{
			// ignored
		}
	}

	/// <inheritdoc />
	public bool IsEnabled(LogLevel logLevel) => true;

	/// <inheritdoc />
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}
