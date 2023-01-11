using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PubNet.API.Contexts;
using PubNet.API.Controllers;
using PubNet.API.Interfaces;
using PubNet.API.Middlewares;
using PubNet.API.Models;
using PubNet.API.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddDbContext<PubNetContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("PubNet"));
    }, ServiceLifetime.Singleton);

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
    builder.Services.AddScoped<IUrlHelper>(services => {
        var actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
        var factory = services.GetRequiredService<IUrlHelperFactory>();
        return factory.GetUrlHelper(actionContext ?? throw new InvalidOperationException("Unable to get ActionContext"));
    });

    builder.Services.AddScoped<IUploadEndpointGenerator, StorageController>();

    builder.Services.AddSingleton<IPackageStorageProvider, LocalPackageStorageProvider>();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    //
    // builder.Services.AddAuthentication(GoogleDefaults.AuthenticationScheme)
    //     .AddGoogle(googleOptions =>
    //     {
    //         var configuration = builder.Configuration;
    //         googleOptions.ClientId = configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException();
    //         googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException();
    //         googleOptions.SignInScheme = JwtBearerDefaults.AuthenticationScheme;
    //     })
    //     .AddJwtBearer()
    //     ;
    //
    // builder.Services
    //     .AddAuthorization(options =>
    //     {
    //         // options.AddPolicy("CreateVersion", policy => policy.RequireAuthenticatedUser());
    //     })
    //     ;

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(builder.Configuration.GetRequiredSection("AllowedOrigins").GetChildren().Select(s => s.Value!).ToArray());
            policy.AllowCredentials();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
    });

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

    // app.UseAuthentication();
    // app.UseAuthorization();

    app.UsePathBase("/api");

    app.UseMiddleware<ClientExceptionFormatterMiddleware>();
    app.UseMiddleware<ApplicationRequestContextEnricherMiddleware>();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    if (ex is not HostAbortedException)
    {
        Log.Fatal(ex, "Application terminated unexpectedly");
    }
    else
    {
        Log.Warning(ex.Message);
    }
}
finally
{
    Log.CloseAndFlush();
}