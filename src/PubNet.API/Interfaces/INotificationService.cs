using PubNet.Database.Models;

namespace PubNet.API.Interfaces;

public interface INotificationService
{
	Task SendWelcomeNotificationAsync(Author author, Uri referer, CancellationToken cancellationToken = default);
}
