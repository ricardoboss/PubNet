using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.StaticFiles;
using PubNet.API.Abstractions;

namespace PubNet.API.Services.Packages.Dart;

public class AspNetMimeTypeProvider : IMimeTypeProvider
{
	private readonly FileExtensionContentTypeProvider contentTypeProvider = new();

	[return: NotNullIfNotNull("fallbackMimeType")]
	public string? GetMimeType(string fileName, string? fallbackMimeType = "application/octet-stream")
	{
		return contentTypeProvider.TryGetContentType(fileName, out var contentType) ? contentType : fallbackMimeType;
	}
}
