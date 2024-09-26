namespace PubNet.API.Abstractions;

public abstract class ApiException(string code, string message, int statusCode) : Exception(message)
{
	public string Code { get; } = code;

	public int StatusCode { get; } = statusCode;
}
