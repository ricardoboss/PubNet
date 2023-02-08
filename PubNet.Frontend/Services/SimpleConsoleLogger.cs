namespace PubNet.Frontend.Services;

public class SimpleConsoleLoggerProvider : ILoggerProvider
{
	/// <inheritdoc />
	public void Dispose()
	{
	}

	/// <inheritdoc />
	public ILogger CreateLogger(string categoryName)
	{
		return new SimpleConsoleLogger(categoryName);
	}
}

public class SimpleConsoleLogger : ILogger
{
	private readonly string _name;

	public LogLevel Minimum { get; set; } = LogLevel.Information;

	public SimpleConsoleLogger(string name)
	{
		_name = name;
	}

	/// <inheritdoc />
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel)) return;

		Console.WriteLine($"[{DateTime.Now}] [{logLevel}] [{_name}] {formatter(state, exception)}");
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
