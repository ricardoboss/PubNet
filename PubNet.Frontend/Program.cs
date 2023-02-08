using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PubNet.Frontend;
using PubNet.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#if DEBUG
builder.Services.AddScoped<SimpleConsoleLoggerProvider>();
await using (var tempProvider = builder.Services.BuildServiceProvider())
{
	builder.Logging.ClearProviders();
	builder.Logging.AddProvider(tempProvider.GetRequiredService<SimpleConsoleLoggerProvider>());
	builder.Logging.SetMinimumLevel(LogLevel.Trace);
	builder.Logging.AddFilter("PubNet.Frontend.Services.FetchLock", LogLevel.None);
	builder.Logging.AddFilter("Microsoft.AspNetCore.Components.RenderTree.*", LogLevel.None);
	builder.Logging.AddFilter("Microsoft.AspNetCore.Components.Routing.Router", LogLevel.Information);
}
#else
builder.Logging.SetMinimumLevel(LogLevel.None);
#endif

builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ApiClient>(sp => new(sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<ILogger<ApiClient>>())
{
#if DEBUG
	BaseAddress = "https://localhost:7171/api/",
#else
	BaseAddress = builder.HostEnvironment.BaseAddress.TrimEnd('/') + "/api/",
#endif
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<PackagesService>();
builder.Services.AddScoped<AnalysisService>();
builder.Services.AddTransient(typeof(FetchLock<>));

var app = builder.Build();
await app.RunAsync();
