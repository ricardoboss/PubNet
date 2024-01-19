using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using PubNet.API.Converter;
using PubNet.API.Middlewares;
using PubNet.API.Services;
using PubNet.ArchiveStorage.BlobStorage;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.LocalFileBlobStorage;
using PubNet.Database.Context;
using PubNet.Database.Entities.Auth;
using PubNet.DocsStorage.Abstractions;
using PubNet.DocsStorage.LocalFileDocsStorage;
using PubNet.PackageStorage.Abstractions;
using Serilog;
using Serilog.Events;
using SignedUrl.Extensions;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	.Enrich.FromLogContext()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	builder.Host.UseSerilog((context, services, configuration) =>
		configuration.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
			.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
			.Enrich.FromLogContext()
			.WriteTo.Console()
	);

#pragma warning disable CS0618 // Type or member is obsolete
	NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
#pragma warning restore CS0618 // Type or member is obsolete

	builder.Services.AddDbContext<PubNetContext>(
		options => options.UseNpgsql(builder.Configuration.GetConnectionString("PubNet"))
	);

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
	builder.Services.TryAddSingleton<IPasswordHasher<Identity>, PasswordHasher<Identity>>();
	builder.Services.AddScoped<PasswordManager>();

	// generates JWT tokens
	builder.Services.AddSingleton<JwtTokenGenerator>();

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

	// package storage
	builder.Services.AddSingleton<IBlobStorage, LocalFileBlobStorage>();
	builder.Services.AddSingleton<IArchiveStorage, BlobArchiveStorage>();
	builder.Services.AddSingleton<IDocsStorage, LocalFileDocsStorage>();

	builder.Services.AddSignedUrl();

	builder.Services.AddControllers()
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
			options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
		});

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

	var app = builder.Build();

	await PubNetContext.RunMigrations(app.Services);

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

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "PubNet API v1");
			c.EnableTryItOutByDefault();
		});
	}

	app.UseHttpsRedirection();

	app.UseCors();

	app.UseResponseCaching();

	app.UseMiddleware<ClientExceptionFormatterMiddleware>();

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
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
