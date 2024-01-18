namespace PubNet.Web.Abstractions;

public interface IPasswordVerifier
{
	Task<bool> VerifyAsync(Guid identityId, string? password, CancellationToken cancellationToken = default);
}
