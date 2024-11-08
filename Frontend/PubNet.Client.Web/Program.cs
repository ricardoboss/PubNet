using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using PubNet.Client.Web;
using PubNet.Client.Services;
using PubNet.Client.Web.Services;
using TextCopy;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Logging
	.SetMinimumLevel(LogLevel.Trace)
	.AddFilter("Microsoft.AspNetCore.Components.*", LogLevel.Information)
	.AddFilter("Microsoft.AspNetCore.Routing.*", LogLevel.Information)
	.AddFilter("Microsoft.AspNetCore.Authorization.*", LogLevel.Information)
	.AddFilter("Microsoft.Extensions.Localization.*", LogLevel.Information)
	;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

ConfigureServices(builder.Services);

await builder.Build().RunAsync();

return;

void ConfigureServices(IServiceCollection services)
{
	AddAuth(services);

	services.InjectClipboard();
	services.AddBrowserLoginTokenStorage();
	services.AddPubNetApiClient(c =>
	{
		var baseAddress = builder.Configuration["BaseAddress"];
		baseAddress ??= builder.HostEnvironment.BaseAddress + "/api/";

		c.BaseAddress = new Uri(baseAddress);
	});

	services.AddMudServices(c =>
	{
		c.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomEnd;
		c.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
	});
}

void AddAuth(IServiceCollection services)
{
	services.AddSingleton<AuthenticationStateProvider, LoginTokenStorageAuthenticationStateProvider>();
	services.AddAuthorizationCore();
}
