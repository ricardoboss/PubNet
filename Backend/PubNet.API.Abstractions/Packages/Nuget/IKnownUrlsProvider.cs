namespace PubNet.API.Abstractions.Packages.Nuget;

public interface IKnownUrlsProvider
{
	string GetRegistrationsBaseUrl();

	string GetPackageBaseAddress();

	string GetPackagePublishUrl();

	string GetSearchAutocompleteServiceUrl();

	string GetSearchQueryServiceUrl();

	string GetVulnerabilityInfoUrl();
}
