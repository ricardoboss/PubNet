using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Npgsql;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.Abstractions.Packages.Nuget;
using PubNet.API.Converter;
using PubNet.API.DTO;
using PubNet.API.Middlewares;
using PubNet.API.Services;
using PubNet.API.Services.Authentication;
using PubNet.API.Services.CQRS.Commands;
using PubNet.API.Services.CQRS.Queries;
using PubNet.API.Services.Packages.Nuget;
using PubNet.ArchiveStorage.BlobStorage;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.LocalFileBlobStorage;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;
using PubNet.DocsStorage.Abstractions;
using PubNet.DocsStorage.LocalFileDocsStorage;
using PubNet.PackageStorage.Abstractions;
using PubNet.Web.Abstractions;
using PubNet.Web.Abstractions.Services;
using PubNet.Web.Services;
using Serilog;
using SignedUrl.Extensions;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.MinimumLevel.Verbose()
	.Enrich.FromLogContext()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	ConfigureServices(builder);

	var app = builder.Build();

	ConfigureHttpPipeline(app);

	app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

	await PubNetContext.RunMigrations(app.Services);

	app.Logger.LogInformation("Application started");

	await app.RunAsync();
}
catch (Exception ex)
{
	if (ex is HostAbortedException)
		Log.Warning("{Message}", ex.Message);
	else
		Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}

return;

void ConfigureServices(WebApplicationBuilder webApplicationBuilder)
{
	ConfigureLogging(webApplicationBuilder);

	ConfigureDatabase(webApplicationBuilder);

	ConfigureAuthentication(webApplicationBuilder);

	ConfigureDynamicUrlGeneration(webApplicationBuilder);

	ConfigurePackageStorage(webApplicationBuilder);

	ConfigureNugetServices(webApplicationBuilder);

	ConfigureControllers(webApplicationBuilder);

	ConfigureSwagger(webApplicationBuilder);

	ConfigureCors(webApplicationBuilder);

	ConfigureHttpServices(webApplicationBuilder);
}

void ConfigureHttpServices(IHostApplicationBuilder builder)
{
	builder.Services.AddDetection();

	builder.Services.AddResponseCaching();
}

void ConfigureCors(WebApplicationBuilder builder)
{
	builder.Services.AddCors(options =>
	{
		options.AddDefaultPolicy(policy =>
		{
			var origins = builder.Configuration
				.GetRequiredSection("AllowedOrigins")
				.GetChildren()
				.Select(s => s.Value!)
				.ToArray();

			policy.WithOrigins(origins);
			policy.AllowCredentials();
			policy.AllowAnyHeader();
			policy.AllowAnyMethod();
		});
	});
}

void ConfigureSwagger(IHostApplicationBuilder builder)
{
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen(o =>
	{
		o.SwaggerDoc("v1", new()
		{
			Title = "PubNet API",
			Description = "An API for Dart and NuGet package hosting",
			Version = "v1",
			License = new()
			{
				Name = "AGPL-3.0",
				Url = new("https://www.gnu.org/licenses/agpl-3.0.en.html"),
			},
		});

		o.InferSecuritySchemes();

		// o.AddSecurityDefinition("Bearer", new()
		// {
		// 	In = ParameterLocation.Header,
		// 	Description = "Please insert JWT with Bearer into field",
		// 	Name = "Authorization",
		// 	Type = SecuritySchemeType.ApiKey
		// });

		o.AddSecurityRequirement(new()
		{
			{
				new()
				{
					Reference = new()
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer",
					},
				},
				Array.Empty<string>()
			},
		});
	});
}

void ConfigureControllers(IHostApplicationBuilder builder)
{
	builder.Services.AddControllers()
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.AddDtoSourceGenerators();

			options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
		});
}

void ConfigureNugetServices(IHostApplicationBuilder builder)
{
	builder.Services.AddScoped<IKnownUrlsProvider, KnownUrlsProvider>();
	builder.Services.AddScoped<INugetServiceIndexProvider, NugetServiceIndexProvider>();
}

void ConfigurePackageStorage(IHostApplicationBuilder builder)
{
	// package storage
	builder.Services.AddSingleton<IBlobStorage, LocalFileBlobStorage>();
	builder.Services.AddSingleton<IArchiveStorage, BlobArchiveStorage>();
	builder.Services.AddSingleton<IDocsStorage, LocalFileDocsStorage>();

	// needed for unauthenticated file uploads
	builder.Services.AddSignedUrl();
}

void ConfigureDynamicUrlGeneration(IHostApplicationBuilder builder)
{
	// needed to dynamically generate uris to controller actions
	builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
	builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
	builder.Services.AddScoped<IUrlHelper>(services =>
	{
		var actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
		var factory = services.GetRequiredService<IUrlHelperFactory>();
		return factory.GetUrlHelper(actionContext ??
			throw new InvalidOperationException("Unable to get ActionContext"));
	});
	builder.Services.AddScoped<IActionTemplateGenerator, ActionTemplateGenerator>();
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
	builder.Services
		.AddAuthentication(o =>
		{
			o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(o =>
		{
			o.SaveToken = true;

			o.TokenValidationParameters = new()
			{
				ValidIssuer = JwtFactory.GetIssuer(builder.Configuration),
				ValidAudience = JwtFactory.GetAudience(builder.Configuration),
				IssuerSigningKey = JwtFactory.GetSecretKey(builder.Configuration),
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
			};
		});

	builder.Services.AddSingleton<IPasswordHasher<Identity>, PasswordHasher<Identity>>();
	builder.Services.AddSingleton<ISecureTokenGenerator, SecureTokenGenerator>();
	builder.Services.AddSingleton<IJwtFactory, JwtFactory>();
	builder.Services.AddSingleton<IGuard, Guard>();

	builder.Services.AddScoped<IAuthorDao, AuthorDao>();
	builder.Services.AddScoped<IIdentityDao, IdentityDao>();

	builder.Services.AddScoped<ITokenDmo, TokenDmo>();
	builder.Services.AddScoped<IAuthorDmo, AuthorDmo>();
	builder.Services.AddScoped<IIdentityDmo, IdentityDmo>();

	builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
	builder.Services.AddScoped<IPasswordVerifier, PasswordVerifier>();
	builder.Services.AddScoped<IAccountService, AccountService>();
	builder.Services.AddScoped<IAuthProvider, HttpAuthProvider>();
	builder.Services.AddScoped<IClientInformationProvider, WangkanaiDetectionClientInformationProvider>();
}

void ConfigureDatabase(WebApplicationBuilder builder)
{
#pragma warning disable CS0618 // Type or member is obsolete
	NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
#pragma warning restore CS0618 // Type or member is obsolete

	builder.Services.AddDbContext<PubNetContext>(
		options => options.UseNpgsql(builder.Configuration.GetConnectionString("PubNet"))
	);
}

void ConfigureLogging(WebApplicationBuilder builder)
{
	builder.Host.UseSerilog((context, services, configuration) =>
		configuration.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
			.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
			.Enrich.FromLogContext()
			.WriteTo.Console()
	);
}

void ConfigureHttpPipeline(WebApplication app)
{
	app.UsePathBase("/api");

	app.UseSerilogRequestLogging(options =>
	{
		options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
		{
			diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
			diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
			diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
		};
	});

	app.UseHttpsRedirection();

	app.UseCors();

	app.UseDetection();

	app.UseResponseCaching();

	app.UseMiddleware<ExceptionFormatterMiddleware>();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	if (!app.Environment.IsDevelopment())
		return;

	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "PubNet API v1");
		c.EnableTryItOutByDefault();
	});
}
