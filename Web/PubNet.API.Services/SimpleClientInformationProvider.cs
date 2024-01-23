using Microsoft.AspNetCore.Http;
using PubNet.API.Abstractions;

namespace PubNet.API.Services;

public class SimpleClientInformationProvider(IHttpContextAccessor contextAccessor) : IClientInformationProvider
{
	private HttpContext Context => contextAccessor.HttpContext ?? new DefaultHttpContext();

	public string IpAddress => Context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

	public string UserAgent => Context.Request.Headers.UserAgent.FirstOrDefault() ?? "unknown";

	public string DeviceType => "Desktop";

	public string Browser => "Chrome";

	public string Platform => "Windows";
}
