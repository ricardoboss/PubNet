using PubNet.Database.Models;

namespace PubNet.API.Interfaces;

public interface INotificationService
{
	Task SendWelcomeNotificationAsync(Author author, CancellationToken cancellationToken = default);

	/// <summary>
	/// Tells the first admin that the instance is set up and now theirs.
	/// </summary>
	Task SendSetupCompletedNotificationAsync(Author author,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends the author a link containing the given password reset token.
	/// </summary>
	Task SendPasswordResetNotificationAsync(Author author, string token,
		CancellationToken cancellationToken = default);
}
