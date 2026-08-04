using PubNet.API.Interfaces;
using PubNet.API.Mails;
using PubNet.Database.Models;
using TRENZ.Lib.RazorMail.Interfaces;
using TRENZ.Lib.RazorMail.Models;

namespace PubNet.API.Services;

public class MailNotificationService(IMailRenderer mailRenderer, IMailClient mailClient) : INotificationService
{
	/// <inheritdoc />
	public Task SendWelcomeNotificationAsync(Author author, Uri referer,
		CancellationToken cancellationToken = default)
	{
		return SendAsync("Mails/WelcomeMail", new WelcomeMailModel
		{
			UserName = author.UserName,
			FrontendUrl = referer,
		}, author, cancellationToken);
	}

	/// <inheritdoc />
	public Task SendSetupCompletedNotificationAsync(Author author, Uri referer,
		CancellationToken cancellationToken = default)
	{
		return SendAsync("Mails/SetupCompletedMail", new SetupCompletedMailModel
		{
			UserName = author.UserName,
			FrontendUrl = referer,
		}, author, cancellationToken);
	}

	/// <inheritdoc />
	public Task SendPasswordResetNotificationAsync(Author author, string token, Uri referer,
		CancellationToken cancellationToken = default)
	{
		return SendAsync("Mails/PasswordResetMail", new PasswordResetMailModel
		{
			UserName = author.UserName,
			ResetUrl = new(referer, $"/reset-password?token={Uri.EscapeDataString(token)}"),
			ValidFor = PasswordResetService.TokenLifetime,
		}, author, cancellationToken);
	}

	private async Task SendAsync<TModel>(string template, TModel model, Author recipient,
		CancellationToken cancellationToken)
	{
		var mailContent = await mailRenderer.RenderAsync(template, model, cancellationToken);

		var mailMessage = new MailMessage
		{
			Content = mailContent,
			Headers = new()
			{
				From = "notifier@pubnet.local",
				Recipients = [recipient.Email],
			},
		};

		await mailClient.SendAsync(mailMessage, cancellationToken);
	}
}
