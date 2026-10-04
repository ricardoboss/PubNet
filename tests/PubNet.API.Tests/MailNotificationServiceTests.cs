using Microsoft.Extensions.Options;
using PubNet.API.Mails;
using PubNet.API.Services;
using PubNet.Database.Models;
using TRENZ.Lib.RazorMail.Interfaces;

namespace PubNet.API.Tests;

public class MailNotificationServiceTests
{
	[Test]
	public async Task SendPasswordResetNotificationAsync_LinksToTheConfiguredFrontend()
	{
		PasswordResetMailModel? model = null;
		var renderer = new Mock<IMailRenderer>();
		renderer
			.Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<PasswordResetMailModel>(),
				It.IsAny<CancellationToken>()))
			.Callback((string _, PasswordResetMailModel m, CancellationToken _) => model = m);

		var service = new MailNotificationService(renderer.Object, Mock.Of<IMailClient>(),
			Options.Create(new FrontendOptions { BaseUrl = "https://pubnet.example.com" }));

		await service.SendPasswordResetNotificationAsync(new() { UserName = "alice", Email = "alice@example.com" },
			"t0ken");

		Assert.That(model?.ResetUrl, Is.EqualTo(new Uri("https://pubnet.example.com/reset-password?token=t0ken")));
	}
}
