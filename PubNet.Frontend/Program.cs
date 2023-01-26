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

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new(builder.Configuration["Api:Base"] ?? throw new("Missing Api:Base value in configuration")),
});

await builder.Build().RunAsync();
