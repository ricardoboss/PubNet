namespace PubNet.Frontend.Services;

public class PackageNotFoundException : Exception
{
	public PackageNotFoundException(string message) : base(message)
	{
	}
}
