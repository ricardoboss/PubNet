using Microsoft.JSInterop;

namespace PubNet.Frontend.Services;

public class SimpleConsoleLoggerProvider : ILoggerProvider
{
	private readonly IJSRuntime _jsRuntime;

	public SimpleConsoleLoggerProvider(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}

	/// <inheritdoc />
	public void Dispose()
	{
	}

	/// <inheritdoc />
	public ILogger CreateLogger(string categoryName)
	{
		return new SimpleConsoleLogger(categoryName, _jsRuntime);
	}
}

public class SimpleConsoleLogger : ILogger
{
	private readonly string _name;
	private readonly IJSRuntime _jsRuntime;

	public LogLevel Minimum { get; set; } = LogLevel.Information;

	public SimpleConsoleLogger(string name, IJSRuntime jsRuntime)
	{
		_name = name;
		_jsRuntime = jsRuntime;
	}

	/// <inheritdoc />
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel) || logLevel == LogLevel.None) return;

		_ = _jsRuntime.InvokeVoidAsync(
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
			$"[{_name}] {formatter(state, exception)}"
		);
	}

	/// <inheritdoc />
	public bool IsEnabled(LogLevel logLevel)
	{
		return Minimum.CompareTo(logLevel) >= 0;
	}

	/// <inheritdoc />
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		return null;
	}
}
