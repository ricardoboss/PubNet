namespace PubNet.Frontend.Services;

public class BaseAddressProvider(string baseAddress)
{
	public string BaseAddress => baseAddress;

	public Uri BaseUri => new Uri(baseAddress);
}
