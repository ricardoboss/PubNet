using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;

namespace PubNet.API.OpenApi;

public class SecurityRequirementsDocumentTransformer : IOpenApiDocumentTransformer
{
	public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken)
	{
		document.Components ??= new();
		document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
		document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
		{
			Name = JwtBearerDefaults.AuthenticationScheme,
			Description = "The bearer token used to authenticate requests.",
			Type = SecuritySchemeType.Http,
			In = ParameterLocation.Header,
			Scheme = JwtBearerDefaults.AuthenticationScheme,
		};

		document.SecurityRequirements ??= [];
		document.SecurityRequirements.Add(
			new()
			{
				{
					new(JwtBearerDefaults.AuthenticationScheme),
					[]
				},
			}
		);

		return Task.CompletedTask;
	}
}
