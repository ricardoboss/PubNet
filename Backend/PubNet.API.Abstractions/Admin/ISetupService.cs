namespace PubNet.API.Abstractions.Admin;

public interface ISetupService
{
	Task<bool> IsSetupCompleteAsync(CancellationToken cancellationToken = default);
}
