namespace PubNet.Frontend.Services;

public class ApiServerException : Exception
{
	public ApiServerException(string message) : base(message)
	{
	}
}
