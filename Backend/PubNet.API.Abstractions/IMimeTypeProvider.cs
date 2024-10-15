using System.Diagnostics.CodeAnalysis;

namespace PubNet.API.Abstractions;

public interface IMimeTypeProvider
{
	[return: NotNullIfNotNull(nameof(fallbackMimeType))]
	string? GetMimeType(string fileName, string? fallbackMimeType = "application/octet-stream");
}
