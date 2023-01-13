using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubNet.API;
using PubNet.API.Contexts;
using PubNet.API.Controllers;
using PubNet.API.Interfaces;
using PubNet.API.Middlewares;
using PubNet.API.Services;
using PubNet.API.Utils;
using PubNet.Models;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

Log.Logger.Information("Bootstrapping app");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
            .Enrich.FromLogContext()
            .WriteTo.Console());

    builder.Services.AddDbContext<PubNetContext>(
        options => { options.UseNpgsql(builder.Configuration.GetConnectionString("PubNet")); },
        ServiceLifetime.Singleton);

    // used to store request-specific data in a single place
    builder.Services.AddScoped<ApplicationRequestContext>();

    // used for verifying and creating password hashes
    builder.Services.AddSingleton<IPasswordHasher<Author>, PasswordHasher<Author>>();

    // used to manage author tokens
    builder.Services.AddSingleton<AuthorTokenDispenser>();

    // data protection is used to encrypt bearer tokens
    builder.Services.AddDataProtection()
        .UseCryptographicAlgorithms(new()
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256,
        });
    builder.Services.AddSingleton<BearerTokenManager>();

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
    builder.Services.AddScoped<IUploadEndpointGenerator, StorageController>();
    builder.Services.AddSingleton<IPackageStorageProvider, LocalPackageStorageProvider>();

    builder.Services.AddControllers();

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

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

    // background worker for cleanup tasks etc
    builder.Services.AddSingleton<DartCli>();
    builder.Services.AddSingleton<WorkerTaskQueue>();
    builder.Services.AddHostedService<Worker>();

    var app = builder.Build();

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
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseResponseCaching();

    // app.UseAuthentication();
    // app.UseAuthorization();

    app.UsePathBase("/api");

    app.UseMiddleware<ClientExceptionFormatterMiddleware>();
    app.UseMiddleware<ApplicationRequestContextEnricherMiddleware>();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    if (ex is HostAbortedException)
    {
        Log.Warning(ex.Message);
    }
    else
    {
        Log.Fatal(ex, "Application terminated unexpectedly");
    }
}
finally
{
    Log.CloseAndFlush();
}
