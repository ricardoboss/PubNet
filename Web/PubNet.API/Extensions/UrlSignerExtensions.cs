using SignedUrl.Abstractions;

namespace PubNet.API.Extensions;

public static class UrlSignerExtensions
{
	public static string GenerateFullyQualified(this IUrlSigner urlSigner, HttpRequest request, string path)
	{
		var scheme = request.Scheme;
		var host = request.Host;

		return urlSigner.Sign($"{scheme}://{host}{path}");
	}
}
