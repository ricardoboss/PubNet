using Microsoft.JSInterop;

namespace PubNet.Frontend.Services;

public class AlertService
{
	private readonly IJSRuntime _jsInterop;

	public AlertService(IJSRuntime jsInterop)
	{
		_jsInterop = jsInterop;
	}

	public async Task Show(string text)
	{
		await _jsInterop.InvokeVoidAsync("alert", text);
	}

	public async Task<bool> Confirm(string text)
	{
		return await _jsInterop.InvokeAsync<bool>("confirm", text);
	}

	public async Task<string?> Prompt(string text)
	{
		return await _jsInterop.InvokeAsync<string?>("prompt", text);
	}
}
