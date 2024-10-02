namespace PubNet.API.Abstractions.Authentication;

public interface IRegistrationsService
{
	Task<bool> AreRegistrationsOpenAsync(CancellationToken cancellationToken = default);
}
