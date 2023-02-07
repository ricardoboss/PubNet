using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PubNet.Frontend;
using PubNet.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<AlertService>();

builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ApiClient>(sp => new(sp.GetRequiredService<HttpClient>())
{
#if DEBUG
	BaseAddress = "https://localhost:7171/api/",
#else
	BaseAddress = builder.HostEnvironment.BaseAddress.TrimEnd('/') + "/api/",
#endif
});

await builder.Build().RunAsync();
