using PubNet.Auth.Models;

namespace PubNet.API.Abstractions.Guard;

public interface IGuardThrowsBuilder
{
	public void Cannot(Scope scope, string? message = null);
	public void CannotAny(IEnumerable<Scope> scopes, string? message = null);
}
