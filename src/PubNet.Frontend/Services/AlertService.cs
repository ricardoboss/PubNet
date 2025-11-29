using Microsoft.JSInterop;

namespace PubNet.Frontend.Services;

public class AlertService(IJSRuntime jsInterop)
{
	public async Task<bool> Confirm(string text)
	{
		return await jsInterop.InvokeAsync<bool>("confirm", text);
	}

	public async Task<string?> Prompt(string text)
	{
		return await jsInterop.InvokeAsync<string?>("prompt", text);
	}
}
