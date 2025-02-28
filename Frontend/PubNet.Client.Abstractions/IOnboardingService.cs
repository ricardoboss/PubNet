namespace PubNet.Client.Abstractions;

public interface IOnboardingService
{
	Task<bool> IsSetupCompleteAsync(CancellationToken cancellationToken = default);
}
