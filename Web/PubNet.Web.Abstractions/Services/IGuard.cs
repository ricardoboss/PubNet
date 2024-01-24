using PubNet.Web.Abstractions.Models;

namespace PubNet.Web.Abstractions.Services;

public interface IGuard
{
	/// <summary>
	/// Checks if the given claim allows the given scope or any prefix with the '*' wildcard.
	/// </summary>
	/// <param name="claim">A scope claim (i.e. a string that contains a list of scopes separated by ' ').</param>
	/// <param name="scope">The scope to check.</param>
	/// <returns>True if the claim allows the scope or a wildcard prefix, false otherwise.</returns>
	bool Allows(ScopesClaim claim, Scope scope);

	/// <summary>
	/// Checks if the given claim denies the given scope or any prefix with the '*' wildcard. Basically the opposite of
	/// <see cref="Allows"/>.
	/// </summary>
	/// <param name="claim">A scope claim (i.e. a string that contains a list of scopes separated by ' ').</param>
	/// <param name="scope">The scope to check.</param>
	/// <returns>True if the claim denies the scope or a wildcard prefix, false otherwise.</returns>
	bool Denies(ScopesClaim claim, Scope scope);
}
