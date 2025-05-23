using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Npgsql;
using PubNet.API;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Archives;
using PubNet.API.Abstractions.Authentication;
using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.API.Abstractions.CQRS.Commands.Packages;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.Abstractions.Guard;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.Abstractions.Packages.Dart.Docs;
using PubNet.API.Converter;
using PubNet.API.DTO;
using PubNet.API.Helpers;
using PubNet.API.Middlewares;
using PubNet.API.Services;
using PubNet.API.Services.Archives;
using PubNet.API.Services.Authentication;
using PubNet.API.Services.CQRS.Commands;
using PubNet.API.Services.CQRS.Commands.Packages;
using PubNet.API.Services.CQRS.Queries;
using PubNet.API.Services.CQRS.Queries.Packages;
using PubNet.API.Services.Guard;
using PubNet.API.Services.Packages.Dart;
using PubNet.API.Swagger;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.LocalFileBlobStorage;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;
using PubNet.DocsStorage.Abstractions;
using PubNet.DocsStorage.LocalFileDocsStorage;
using PubNet.PackageStorage.Abstractions;
using PubNet.PackageStorage.BlobStorage;
using Serilog;
using SignedUrl.Extensions;

const string openApiDocumentName = "openapi";

if (ApiDescriptionToolDetector.IsToolInvocation())
{
	HandleApiDescriptionToolInvocation();

	return;
}

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.MinimumLevel.Verbose()
	.Enrich.FromLogContext()
	.CreateBootstrapLogger()!;

try
{
	var builder = WebApplication.CreateBuilder(args);

	ConfigureServices(builder);

	var app = builder.Build();

	app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
	app.Logger.LogInformation("Version: {Version}", ThisAssembly.AssemblyInformationalVersion);

	ConfigureHttpPipeline(app);

	await PubNet2Context.RunMigrations(app.Services);

	app.Logger.LogInformation("Application started");

	await app.RunAsync();
}
catch (Exception e) when (e is HostAbortedException or OperationCanceledException or TaskCanceledException ||
	e.GetType().Name is "StopTheHostException")
{
	Log.Information("Application terminated gracefully");
}
catch (Exception e)
{
	Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}

return;

void HandleApiDescriptionToolInvocation()
{
	var builder = WebApplication.CreateBuilder(args);

	ConfigureServices(builder);

	_ = builder.Build();
}

void ConfigureServices(WebApplicationBuilder builder)
{
	ConfigureLogging(builder);

	ConfigureDatabase(builder);

	ConfigureAuthentication(builder);

	ConfigureDataServices(builder);

	ConfigureDynamicUrlGeneration(builder);

	ConfigurePackageStorage(builder);

	ConfigureDartServices(builder);

	ConfigureCommonServices(builder);

	ConfigureControllers(builder);

	ConfigureSwagger(builder);

	ConfigureCors(builder);

	ConfigureHttpServices(builder);
}

void ConfigureHttpServices(IHostApplicationBuilder builder)
{
	builder.Services.AddDetection();

	builder.Services.AddResponseCaching();

	builder.Services.AddSingleton<ExceptionFormatterMiddleware>();
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
		o.SwaggerDoc(openApiDocumentName, new OpenApiInfo
		{
			Title = "PubNet API",
			Description = "An API for Dart and NuGet package hosting",
			Version = ThisAssembly.AssemblyVersion,
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
		});

		var bearerTokenScheme = new OpenApiSecurityScheme
		{
			Name = JwtBearerDefaults.AuthenticationScheme,
			Description = "The bearer token used to authenticate requests.",
			Scheme = JwtBearerDefaults.AuthenticationScheme,
			Type = SecuritySchemeType.Http,
			In = ParameterLocation.Header,
		};

		o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerTokenScheme);

		o.OperationFilter<CommonErrorsOperationFilter>();
		o.OperationFilter<SecurityRequirementsOperationFilter>();
	});
}

void ConfigureControllers(IHostApplicationBuilder builder)
{
	builder.Services
		.AddControllers()
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.AddDtoSourceGenerators();

			options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
		});
}

void ConfigureDartServices(IHostApplicationBuilder builder)
{
	builder.Services.AddScoped<IDartPackageUploadService, DartPackageUploadService>();
	builder.Services.AddScoped<IDartPackageArchiveProvider, DartPackageArchiveProvider>();
	builder.Services.AddScoped<IDartPackageVersionAnalysisProvider, DartPackageVersionAnalysisProvider>();
	builder.Services.AddScoped<IDartPackageVersionDocsProviderFactory, DartPackageVersionDocsProviderFactory>();
}

void ConfigureCommonServices(IHostApplicationBuilder builder)
{
	builder.Services.AddScoped<IMimeTypeProvider, AspNetMimeTypeProvider>();
}

void ConfigurePackageStorage(IHostApplicationBuilder builder)
{
	builder.Services.AddSingleton<IArchiveStorage, BlobArchiveStorage>();
	builder.Services.AddSingleton<IDocsStorage, LocalFileDocsStorage>();

	// register blob storages with a key
	builder.Services.AddKeyedSingleton<IBlobStorage, LocalFileBlobStorage>(LocalFileBlobStorage.ServiceKey);

	// default blob storage is local file storage
	builder.Services.AddTransient<IBlobStorage>(sp =>
		sp.GetRequiredKeyedService<IBlobStorage>(LocalFileBlobStorage.ServiceKey));

	// needed for unauthenticated file uploads
	builder.Services.AddSignedUrl();

	// needed for extracting archives
	builder.Services.AddSingleton<IArchiveReader, TempDirExtractingArchiveReader>();
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
	builder.Services.AddSingleton<BearerEventsHandler>();

	builder.Services
		.AddAuthentication(o =>
		{
			o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(o =>
		{
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

			o.EventsType = typeof(BearerEventsHandler);
		});

	builder.Services.AddSingleton<IPasswordHasher<Identity>, PasswordHasher<Identity>>();
	builder.Services.AddSingleton<ISecureTokenGenerator, SecureTokenGenerator>();
	builder.Services.AddSingleton<IJwtFactory, JwtFactory>();
	builder.Services.AddSingleton<IGuard, Guard>();
	builder.Services.AddScoped<IRegistrationsService, SeedingAndConfiguredRegistrationsService>();

	builder.Services.AddSingleton<ScopeGuardMiddleware>();
}

void ConfigureDataServices(IHostApplicationBuilder builder)
{
	builder.Services.AddScoped<IAuthorDao, AuthorDao>();
	builder.Services.AddScoped<IIdentityDao, IdentityDao>();
	builder.Services.AddScoped<IDartPackageDao, DartPackageDao>();
	builder.Services.AddScoped<ITokenDao, TokenDao>();

	builder.Services.AddScoped<ITokenDmo, TokenDmo>();
	builder.Services.AddScoped<IAuthorDmo, AuthorDmo>();
	builder.Services.AddScoped<IIdentityDmo, IdentityDmo>();
	builder.Services.AddScoped<IDartPackageDmo, DartPackageDmo>();

	builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
	builder.Services.AddScoped<IPasswordVerifier, DefaultPasswordVerifier>();
	builder.Services.AddScoped<IAccountService, DefaultAccountService>();
	builder.Services.AddScoped<IAuthProvider, HttpAuthProvider>();
	builder.Services.AddScoped<IClientInformationProvider, WangkanaiDetectionClientInformationProvider>();
}

void ConfigureDatabase(WebApplicationBuilder builder)
{
#pragma warning disable CS0618 // Type or member is obsolete
	NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
#pragma warning restore CS0618 // Type or member is obsolete

	builder.Services.AddDbContext<PubNet2Context>(
		options => options
			.UseLazyLoadingProxies()
			.UseNpgsql(builder.Configuration.GetConnectionString("PubNet"))
	);
}

void ConfigureLogging(WebApplicationBuilder builder)
{
	builder.Host.UseSerilog((context, services, configuration) =>
		configuration.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)!
			.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
			.Enrich.FromLogContext()
			.WriteTo.Console()
	);
}

void ConfigureHttpPipeline(WebApplication app)
{
	app.UseRewriter(new RewriteOptions().Add(new PubClientRewriteRule()));

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

	app.UseMiddleware<ExceptionFormatterMiddleware>();

	app.UseCors();

	app.UseDetection();

	app.UseAuthentication();
	app.UseAuthorization();

	app.UseMiddleware<ScopeGuardMiddleware>();

	app.UseResponseCaching();

	app.MapControllers();

	app.MapSwagger("/.well-known/{documentName}.{extension:regex(^(json|ya?ml)$)}");

	if (!app.Environment.IsDevelopment())
		return;

	app.UseSwaggerUI(c =>
	{
		c.EnableTryItOutByDefault();
		c.EnablePersistAuthorization();
		c.SwaggerEndpoint($"/.well-known/{openApiDocumentName}.json", $"PubNet API {ThisAssembly.AssemblyVersion}");
	});
}
