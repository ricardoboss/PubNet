﻿using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace PubNet.API.OpenApi;

public class PubNetDocumentTransformer : IOpenApiDocumentTransformer
{
	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken)
	{
		document.Info = new OpenApiInfo
		{
			Title = "PubNet API",
			Description = "An API for Dart and NuGet package hosting",
			Version = GitVersionInformation.MajorMinorPatch,
			Contact = new OpenApiContact
			{
				Name = "GitHub",
				Url = new Uri("https://github.com/ricardoboss/PubNet/issues"),
			},
			License = new OpenApiLicense
			{
				Name = "AGPL-3.0",
				Url = new Uri("https://www.gnu.org/licenses/agpl-3.0.en.html"),
			},
		};

		return Task.CompletedTask;
	}
}