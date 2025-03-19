using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace PubNet.API.OpenApi;

public class PubNetDocumentTransformer : IOpenApiDocumentTransformer
{
	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken)
	{
		document.Info = new()
		{
			Title = "PubNet API",
			Description = "An API for Dart and NuGet package hosting",
			Version = ThisAssembly.MajorMinorPatch,
			Contact = new()
			{
				Name = "GitHub",
				Url = new("https://github.com/ricardoboss/PubNet/issues"),
			},
			License = new()
			{
				Name = "AGPL-3.0",
				Url = new("https://www.gnu.org/licenses/agpl-3.0.en.html"),
			},
		};

		return Task.CompletedTask;
	}
}
