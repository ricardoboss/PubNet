using PubNet.API.Interfaces;
using PubNet.API.Mails;
using PubNet.Database.Models;
using TRENZ.Lib.RazorMail.Interfaces;
using TRENZ.Lib.RazorMail.Models;

namespace PubNet.API.Services;

public class MailNotificationService(IMailRenderer mailRenderer, IMailClient mailClient) : INotificationService
{
	public async Task SendWelcomeNotificationAsync(Author author, Uri referer, CancellationToken cancellationToken = default)
	{
		var mailContent = await mailRenderer.RenderAsync("Mails/WelcomeMail", new WelcomeMailModel
		{
			UserName = author.UserName,
			FrontendUrl = referer,
		}, cancellationToken);

		var mailMessage = new MailMessage
		{
			Content = mailContent,
			Headers = new()
			{
				From = "notifier@pubnet.local",
				Recipients = [author.Email],
			},
		};

		await mailClient.SendAsync(mailMessage, cancellationToken);
	}
}
