using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
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

    builder.Services.AddDbContext<PubNetContext>(
        options => options
            .UseNpgsql(builder.Configuration.GetConnectionString("PubNet"))
            .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
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
        // .AddGoogle(googleOptions =>
        // {
        //     googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new(
        //         "Missing Authentication:Google:ClientId in configuration. See https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-7.0#store-the-google-client-id-and-secret");
        //     googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new(
        //         "Missing Authentication:Google:ClientSecret in configuration. See https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-7.0#store-the-google-client-id-and-secret");
        //     googleOptions.SaveTokens = true;
        //     // googleOptions.Events.OnCreatingTicket = ctx =>
        //     // {
        //     //     var tokens = ctx.Properties.GetTokens().ToList();
        //     //
        //     //     tokens.Add(new()
        //     //     {
        //     //         Name = "TicketCreated",
        //     //         Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
        //     //     });
        //     //
        //     //     ctx.Properties.StoreTokens(tokens);
        //     //
        //     //     return Task.CompletedTask;
        //     // };
        // })
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

    // used to store request-specific data in a single place
    builder.Services.AddScoped<ApplicationRequestContext>();

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
    builder.Services.AddSingleton<EndpointHelper>();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(o =>
    {
        const string definitionName = "Bearer";

        o.AddSecurityDefinition(definitionName, new()
        {
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Scheme = "Bearer",
        });

        o.AddSecurityRequirement(new()
        {
            {
                new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = definitionName } },
                new List<string>()
            },
        });
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

    // background worker for cleanup tasks etc
    builder.Services.AddSingleton<DartCli>();
    builder.Services.AddSingleton<WorkerTaskQueue>();
    builder.Services.AddHostedService<Worker>();

    var app = builder.Build();

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
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseResponseCaching();

    app.UseMiddleware<ClientExceptionFormatterMiddleware>();
    app.UseMiddleware<ApplicationRequestContextEnricherMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

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