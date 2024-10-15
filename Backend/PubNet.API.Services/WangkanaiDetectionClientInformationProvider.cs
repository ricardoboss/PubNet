using Microsoft.AspNetCore.Http;
using PubNet.API.Abstractions;
using Wangkanai.Detection.Services;

namespace PubNet.API.Services;

public class WangkanaiDetectionClientInformationProvider(IHttpContextAccessor contextAccessor, IDetectionService detectionService) : IClientInformationProvider
{
	private HttpContext Context => contextAccessor.HttpContext ?? new DefaultHttpContext();

	public string IpAddress => Context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

	public string UserAgent => detectionService.UserAgent.ToString();

	public string DeviceType => detectionService.Device.Type.ToString();

	public string Browser => detectionService.Browser.Name.ToString();

	public string Platform => detectionService.Platform.Name.ToString();
}
