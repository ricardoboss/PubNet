using Microsoft.JSInterop;

namespace PubNet.Frontend.Services;

public class ClipboardService(IJSRuntime jsInterop)
{
	public async Task WriteText(string text)
	{
		await jsInterop.InvokeVoidAsync("navigator.clipboard.writeText", text);
	}
}
