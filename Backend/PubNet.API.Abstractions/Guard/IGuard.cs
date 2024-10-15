using PubNet.Auth.Models;

namespace PubNet.API.Abstractions.Guard;

public interface IGuard
{
	/// <summary>
	/// Checks if the given claim allows the given scope or any prefix with the '*' wildcard.
	/// </summary>
	/// <param name="claim">A scope claim (i.e. a string that contains a list of scopes separated by ' ').</param>
	/// <param name="targetScope">The scope to check.</param>
	/// <returns>True if the claim allows the scope or a wildcard prefix, false otherwise.</returns>
	bool Allows(ScopesClaim claim, Scope targetScope);

	/// <summary>
	/// Checks if the claimed role is allowed to perform the given action for the target role.
	/// </summary>
	/// <param name="claimedRole">The role that is claimed by the user.</param>
	/// <param name="toActAs">The role that is targeted by the action.</param>
	/// <returns>True if the claimed role is allowed to perform the action for the target role, false otherwise.</returns>
	bool Allows(Role claimedRole, Role toActAs);
}
