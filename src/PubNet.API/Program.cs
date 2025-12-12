using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubNet.API.Controllers;
using PubNet.API.Converter;
using PubNet.API.Helpers;
using PubNet.API.Interfaces;
using PubNet.API.Middlewares;
using PubNet.API.OpenApi;
using PubNet.API.Services;
using PubNet.Common.Interfaces;
using PubNet.Common.Models;
using PubNet.Common.Services;
using PubNet.Database;
using PubNet.Database.Models;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using TRENZ.Lib.RazorMail.Extensions;
using TRENZ.Lib.RazorMail.MailKit.Extensions;

const string openApiDocumentName = "openapi";

if (ApiDescriptionToolDetector.IsToolInvocation())
{
	HandleApiDescriptionToolInvocation();

	return;
}

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	.Enrich.FromLogContext()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	ConfigureServices(builder);

	var app = builder.Build();

	ConfigureHttpPipeline(app);

	await PubNetContext.RunMigrations(app.Services);

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
	builder.Host.UseSerilog((context, services, configuration) =>
		configuration.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
			.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
			.Enrich.FromLogContext()
			.WriteTo.Console()
	);

	builder.Services.AddDbContext<PubNetContext>(
		options => options.UseNpgsql(builder.Configuration.GetConnectionString("PubNet"), o =>
		{
			o.ConfigureDataSource(ds =>
			{
				ds.EnableDynamicJson(jsonClrTypes: [typeof(PubSpec), typeof(PubSpecScreenshot)]);
			});
		})
	);

	builder.Services
		.AddIdentityCore<Author>()
		.AddEntityFrameworkStores<PubNetContext>();

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
				ValidIssuer = JwtTokenGenerator.GetIssuer(builder.Configuration),
				ValidAudience = JwtTokenGenerator.GetAudience(builder.Configuration),
				IssuerSigningKey = JwtTokenGenerator.GetSecretKey(builder.Configuration),
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = false,
				ValidateIssuerSigningKey = true,
			};
		});

	// used for verifying and creating password hashes
	builder.Services.TryAddSingleton<IPasswordHasher<Author>, PasswordHasher<Author>>();
	builder.Services.AddScoped<PasswordManager>();

	// generates JWT tokens
	builder.Services.AddSingleton<JwtTokenGenerator>();

	// mails
	builder.Services.AddRazorMailRenderer();
	builder.Services.AddMailKitMailClient();
	builder.Services.AddScoped<INotificationService, MailNotificationService>();

	// used to store request-specific data in a single place
	builder.Services.AddScoped<ApplicationRequestContext>();

	// package storage
	builder.Services.AddScoped<IUploadEndpointGenerator, StorageController>();
	builder.Services.AddPackageStorageProviderOptions();
	builder.Services.AddSingleton<IPackageStorageProvider, LocalPackageStorageProvider>();
	builder.Services.AddSingleton<EndpointHelper>();

	// mirror for pub.dev
	builder.Services.AddHttpClient(PubDevPackageProvider.ClientName, options =>
	{
		options.BaseAddress = new("https://pub.dev/api/");
		options.DefaultRequestHeaders.Accept.Add(new("application/json"));
		options.DefaultRequestHeaders.UserAgent.Clear();
		options.DefaultRequestHeaders.UserAgent.Add(new("pub-net-mirror", "1.0.0"));
		options.DefaultRequestHeaders.UserAgent.Add(new("(+https://github.com/ricardoboss/PubNet)"));
	});
	builder.Services.AddSingleton<PubDevPackageProvider>();

	builder.Services.AddControllers()
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
			options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
		});

	builder.Services.AddOpenApi(openApiDocumentName, o =>
	{
		o.AddDocumentTransformer<PubNetDocumentTransformer>();
		o.AddDocumentTransformer<SecurityRequirementsDocumentTransformer>();
		o.AddDocumentTransformer<ErrorsOperationTransformer>();
		o.AddDocumentTransformer<NullableRelaxingDocumentTransformer>();

		o.AddOperationTransformer<CleanupOperationTransformer>();
		o.AddOperationTransformer<ErrorsOperationTransformer>();
		o.AddOperationTransformer<AuthMetadataTransformer>();
	});

	builder.Services.AddCors(options =>
	{
		options.AddDefaultPolicy(policy =>
		{
			policy.WithOrigins(builder.Configuration.GetRequiredSection("AllowedOrigins").GetChildren()
				.Select(s => s.Value!).ToArray());
			policy.AllowCredentials();
			policy.AllowAnyHeader();
			policy.AllowAnyMethod();
		});
	});

	builder.Services.AddResponseCaching();
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

	app.UseResponseCaching();

	app.MapOpenApi("/.well-known/{documentName}.{extension:regex(^(json|ya?ml)$)}");

	app.UseMiddleware<ClientExceptionFormatterMiddleware>();
	app.UseMiddleware<ApplicationRequestContextEnricherMiddleware>();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	if (app.Environment.IsDevelopment())
	{
		app.MapScalarApiReference(c =>
		{
			c.Telemetry = false;
			c.Theme = ScalarTheme.DeepSpace;
			c.OpenApiRoutePattern = $"/.well-known/{openApiDocumentName}.json";
		});
	}
}
