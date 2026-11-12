using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using PubNet.API.Services;

namespace PubNet.API.Tests;

public class EndpointHelperTests
{
	[Test]
	public void GenerateFullyQualified_IgnoresUnprocessedForwardedHeaders()
	{
		var context = new DefaultHttpContext();
		context.Request.Scheme = "https";
		context.Request.Host = new("pubnet.example.com");
		context.Request.Headers["X-Forwarded-Proto"] = "http";
		context.Request.Headers["X-Forwarded-Host"] = "attacker.example";
		context.Request.Headers["X-Forwarded-Port"] = "8080";

		var helper = new EndpointHelper(Mock.Of<IDataProtectionProvider>());

		var url = helper.GenerateFullyQualified(context.Request, "/api/storage/upload");

		Assert.That(url, Is.EqualTo("https://pubnet.example.com/api/storage/upload"));
	}
}
