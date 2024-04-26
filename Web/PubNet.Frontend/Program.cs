using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using PubNet.Client.Generated;
using PubNet.Frontend;
using PubNet.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// set up logging
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

// API client services
builder.Services
	.AddScoped<AccessTokenProvider>()
	.AddTransient<IAccessTokenProvider>(sp => sp.GetRequiredService<AccessTokenProvider>())
	.AddScoped<IAuthenticationProvider, BaseBearerTokenAuthenticationProvider>()
	.AddScoped<HttpClient>(sp =>
	{
		return new HttpClient()
		{
#if DEBUG
			BaseAddress = new Uri("https://localhost:7171/api/", UriKind.Absolute),
#else
			BaseAddress = new Uri(builder.HostEnvironment.BaseAddress.TrimEnd('/') + "/api/", UriKind.Absolute),
#endif
		};
	})
	.AddScoped<IRequestAdapter, HttpClientRequestAdapter>()
	.AddScoped<ApiClient>();

// set up Blazorise
builder.Services
	.AddBlazorise(options => { options.Immediate = true; })
	.AddBulmaProviders()
	.AddFontAwesomeIcons();

// set up common services
builder.Services
	.AddBlazoredLocalStorage()
	.AddScoped<AuthenticationService>()
	.AddSingleton<ClipboardService>()
	.AddSingleton<AlertService>()
	.AddScoped<PackagesService>()
	.AddScoped<AnalysisService>()
	.AddTransient(typeof(FetchLock<>));

var app = builder.Build();
await app.RunAsync();
