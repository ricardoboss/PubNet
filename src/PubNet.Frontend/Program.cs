using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PubNet.Frontend;
using PubNet.Frontend.Services;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Extensions;
using PubNet.SDK.Services;

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
	builder.Logging.AddFilter("Microsoft.AspNetCore.Components.RenderTree", LogLevel.Error);
}
#else
builder.Logging.SetMinimumLevel(LogLevel.Error);
#endif

// API client services
var bap = new BaseAddressProvider(
#if DEBUG
	"https://localhost:7171/api/"
#else
	builder.HostEnvironment.BaseAddress.TrimEnd('/') + "/api/"
#endif
);
builder.Services.AddSingleton(bap);
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ILoginTokenStorage, BrowserLoginTokenStorage>();
builder.Services.AddPubNetApi((sp, c) =>
{
	var bap = sp.GetRequiredService<BaseAddressProvider>();

	c.BaseAddress = bap.BaseUri;
});

// set up Blazorise
builder.Services
	.AddBlazorise(options =>
	{
		options.Immediate = true;
		options.ProductToken = "CjxRBXB/Ngg9UQFwfz01BlEAc3g0CT9TAXB/NQw/bjoNJ2ZdYhBVCCo/DTtRPUsNalV8Al44B2ECAWllMit3cWhZPUsCbFtpDUMkGnxIaVlzLiNoTWIKRDhDD2dTJ3EVD0JqRSdvHgNEYFM8Yg4ZVmdTWQFxfjU1BjxvABtRd08sfRECQGxJPG8MD11nUzF/Fh1aZzZSAHF+CDJTPHMJD1dsXzxvDA9dZ1MxfxYdWmc2UgBMRFpnQCpjFRhMfVs8bwwPXWdTMX8WHVpnNlIAcX4IMlM8ZBMLQG5FJmceEUh5VDxvEwFSa1M8CnB+NTUGLHIwGzdaNRNYFB1XaXpUfQQgX1RmFkUFZWBtYhlcDT08aGEAXSg+Lk9cVGIodlVzbQlTKns8Tms1XC8CblA/MEgKFDAJbQsINyxXc08RBRV6UVRqBxsweDENeQpXIiozbzsgZDEUR1RFNBsQflUNZRZXERRJVDVXRCQPNExlTF8CIGx0OwV7dD1IbWssd3F4MxNJG3ssdnMAeFBEKQoyfUkyAzkKd1NpJ2Iicw==";
	})
	.AddBulmaProviders()
	.AddFontAwesomeIcons();

// set up common services
builder.Services
	.AddSingleton<TextGenerator>()
	.AddScoped<ClipboardService>()
	.AddScoped<AlertService>();

var app = builder.Build();

app.Services.GetRequiredService<ILogger<Program>>().LogInformation("Using base address {BaseAddress}", bap.BaseUri);

await app.RunAsync();
