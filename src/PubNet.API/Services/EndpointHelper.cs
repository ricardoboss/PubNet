using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.DataProtection;

namespace PubNet.API.Services;

public class EndpointHelper(IDataProtectionProvider dataProtectionProvider)
{
	private const string DefaultDigestKey = "d";

	private IDataProtector Protector => dataProtectionProvider.CreateProtector(nameof(EndpointHelper));

	public string GenerateFullyQualified(HttpRequest request, string endpoint,
		IDictionary<string, string?>? queryParams = null)
	{
		// X-Forwarded-* headers are applied by the forwarded headers middleware, which only trusts known proxies
		var builder = new UriBuilder
		{
			Scheme = request.Scheme,
			Host = request.Host.Host,
			Path = endpoint,
		};

		if (request.Host.Port.HasValue)
			builder.Port = request.Host.Port.Value;

		if (queryParams is not null)
			builder.Query = QueryString.Create(queryParams).ToUriComponent();

		return builder.ToString();
	}

	public string SignEndpoint(string endpoint, IDictionary<string, string?>? queryParams = null,
		string digestKey = DefaultDigestKey)
	{
		var uri = new UriBuilder(endpoint);

		var query = HttpUtility.ParseQueryString(uri.Query);
		if (queryParams is not null)
			foreach (var (key, value) in queryParams)
				query[key] = value;

		var queryString = query.ToString()!;
		var queryStringBytes = Encoding.UTF8.GetBytes(queryString);
		var digestBytes = SHA256.HashData(queryStringBytes);
		var securedQueryStringBytes = Protector.Protect(digestBytes);
		var digestString = Convert.ToBase64String(securedQueryStringBytes);
		query[digestKey] = digestString;

		uri.Query = query.ToString();

		return uri.ToString();
	}

	public bool ValidateSignature(string query, string digestKey = DefaultDigestKey)
	{
		var collection = HttpUtility.ParseQueryString(query);
		if (collection[digestKey] == null)
			return false;

		var digestString = collection[digestKey];
		if (digestString is null)
			return false;

		collection.Remove(digestKey);

		var actualQueryString = collection.ToString()!;
		var actualQueryStringBytes = Encoding.UTF8.GetBytes(actualQueryString);
		var actualDigestBytes = SHA256.HashData(actualQueryStringBytes);

		var givenSecuredDigestBytes = Convert.FromBase64String(digestString);
		var givenDigestBytes = Protector.Unprotect(givenSecuredDigestBytes);

		return actualDigestBytes.SequenceEqual(givenDigestBytes);
	}
}
