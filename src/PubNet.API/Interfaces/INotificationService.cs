using PubNet.Database.Models;

namespace PubNet.API.Interfaces;

public interface INotificationService
{
	Task SendWelcomeNotificationAsync(Author author, Uri referer, CancellationToken cancellationToken = default);

	/// <summary>
	/// Tells the first admin that the instance is set up and now theirs.
	/// </summary>
	Task SendSetupCompletedNotificationAsync(Author author, Uri referer,
		CancellationToken cancellationToken = default);
}
