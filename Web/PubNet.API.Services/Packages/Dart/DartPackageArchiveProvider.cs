using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PubNet.API.Abstractions.Packages.Dart;

namespace PubNet.API.Services.Packages.Dart;

public class DartPackageArchiveProvider(IHttpContextAccessor contextAccessor, LinkGenerator linkGenerator) : IDartPackageArchiveProvider
{
	public async Task<Stream?> GetArchiveContentAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public (Uri url, string sha256) GetArchiveUriAndHash(string name, string version)
	{
		var uri = GetArchiveUri(name, version);
		var hash = GetArchiveHash(name, version);

		return (uri, hash);
	}

	private string GetArchiveHash(string name, string version)
	{
		// TODO(rbo): Implement getting archive hash
		return string.Empty;
	}

	private Uri GetArchiveUri(string name, string version)
	{
		var path = linkGenerator.GetPathByAction("GetArchive", "DartPackagesByNameAndVersion", new { name, version });

		var request = contextAccessor.HttpContext?.Request;
		if (request == null)
			throw new InvalidOperationException("Cannot generate URL without an active HTTP context.");

		var builder = new UriBuilder
		{
			Scheme = request.Scheme,
			Host = request.Host.Host,
			Port = request.Host.Port ?? 80,
			Path = path,
		};

		return builder.Uri;
	}
}
