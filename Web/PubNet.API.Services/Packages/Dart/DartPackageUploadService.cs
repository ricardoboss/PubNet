using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.Packages.Dart;

public class DartPackageUploadService(IHttpContextAccessor contextAccessor, LinkGenerator linkGenerator) : IDartPackageUploadService
{
	public Task<DartNewVersionDto> CreateNewAsync(Token token, CancellationToken cancellationToken = default)
	{
		var url = GetUriForUploadEndpoint();
		var fields = new Dictionary<string, string>
		{
			{ "author-id", token.Identity.Author.Id.ToString() },
		};

		Debug.Assert(url.IsAbsoluteUri, "URL must be absolute to ensure it is not tied to a specific host");

		var dto = new DartNewVersionDto
		{
			Url = url.ToString(),
			Fields = fields,
		};

		return Task.FromResult(dto);
	}

	private Uri GetUriForUploadEndpoint()
	{
		var path = linkGenerator.GetPathByAction("Upload", "DartStorage");
		if (path is null)
			throw new InvalidOperationException("Could not generate URL for upload endpoint");

		var context = contextAccessor.HttpContext;
		if (context is null)
			throw new InvalidOperationException("Could not determine host for upload endpoint");

		var builder = new UriBuilder
		{
			Scheme = context.Request.Scheme,
			Host = context.Request.Host.Host,
			Port = context.Request.Host.Port ?? 80,
			Path = path,
		};

		return builder.Uri;
	}
}
