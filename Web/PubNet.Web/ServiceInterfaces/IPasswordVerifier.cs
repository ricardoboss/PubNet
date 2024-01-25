namespace PubNet.Web.ServiceInterfaces;

public interface IPasswordVerifier
{
	Task<bool> VerifyAsync(Guid identityId, string? password, CancellationToken cancellationToken = default);
}
