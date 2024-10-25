using PubNet.Auth.Models;

namespace PubNet.API.Services.Guard;

public class MissingScopeException : UnauthorizedAccessException
{
	public IReadOnlyList<Scope> AvailableScopes { get; }

	public IReadOnlyList<Scope> MissingScopes { get; }

	public MissingScopeException(IReadOnlyList<Scope> availableScopes, Scope missingScope, string? message = null) :
		base(message ??
			$"Scope {missingScope} is missing available claims")
	{
		AvailableScopes = availableScopes;
		MissingScopes = [missingScope];
	}

	public MissingScopeException(IReadOnlyList<Scope> availableScopes, IReadOnlyList<Scope> missingScopes,
		string? message = null) : base(message ??
		$"Scopes {string.Join(", ", missingScopes)} are missing in available claims")
	{
		AvailableScopes = availableScopes;
		MissingScopes = missingScopes;
	}
}
