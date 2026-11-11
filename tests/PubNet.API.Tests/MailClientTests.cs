using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TRENZ.Lib.RazorMail.Interfaces;
using TRENZ.Lib.RazorMail.MailKit.Extensions;
using TRENZ.Lib.RazorMail.Models;

namespace PubNet.API.Tests;

public class MailClientTests
{
	/// <summary>
	/// Guards the MailKit/MimeKit versions against the ones the mail client was compiled for: a mismatch fails
	/// while loading the assemblies, long before a connection is attempted.
	/// </summary>
	[Test]
	public async Task TestReachesTheSmtpServer()
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["SmtpAccount:Host"] = IPAddress.Loopback.ToString(),
				["SmtpAccount:Port"] = GetUnusedPort().ToString(),
				["SmtpAccount:TLS"] = "false",
			})
			.Build();

		await using var services = new ServiceCollection()
			.AddSingleton<IConfiguration>(configuration)
			.AddLogging()
			.AddMailKitMailClient()
			.BuildServiceProvider();

		var mailClient = services.GetRequiredService<IMailClient>();

		var message = new MailMessage
		{
			Content = new() { Subject = "Subject", HtmlBody = "<p>Body</p>" },
			Headers = new()
			{
				From = "notifier@pubnet.local",
				Recipients = ["author@pubnet.local"],
			},
		};

		await Assert.ThrowsAsync<SocketException>(() => mailClient.SendAsync(message));
	}

	private static int GetUnusedPort()
	{
		using var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();

		return ((IPEndPoint)listener.LocalEndpoint).Port;
	}
}
