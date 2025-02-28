namespace PubNet.API.Abstractions;

public interface ISetupService
{
	Task<bool> IsSetupCompleteAsync(CancellationToken cancellationToken = default);
}
