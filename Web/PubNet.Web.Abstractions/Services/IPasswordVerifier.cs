namespace PubNet.Web.Abstractions.Services;

public interface IPasswordVerifier
{
	Task<bool> VerifyAsync(Guid identityId, string? password, CancellationToken cancellationToken = default);
}
