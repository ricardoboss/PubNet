using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

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
			Version = typeof(Program).Assembly.GetName().Version!.ToString(),
			Contact = new()
			{
				Name = "GitHub",
				Url = new("https://github.com/ricardoboss/PubNet/issues"),
			},
			License = new()
			{
				Name = "Apache License",
				Url = new("https://opensource.org/license/apache-2-0"),
			},
		};

		return Task.CompletedTask;
	}
}
