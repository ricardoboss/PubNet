using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
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

    builder.Services.AddSingleton<AuthorTokenDispenser>();

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

    // app.UseAuthentication();
    // app.UseAuthorization();

    app.UsePathBase("/api");

    app.MapControllers();

    app.UseMiddleware<ApplicationRequestContextEnricherMiddleware>();
    app.UseMiddleware<ClientExceptionFormatterMiddleware>();

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