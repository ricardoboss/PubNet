namespace PubNet.API.Abstractions.Packages.Nuget;

public interface IKnownUrlsProvider
{
	public string GetRegistrationsBaseUrl();

	public string GetPackageBaseAddress();

	public string GetPackagePublishUrl();
}
