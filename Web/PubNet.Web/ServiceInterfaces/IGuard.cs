﻿using PubNet.Web.Models;

namespace PubNet.Web.ServiceInterfaces;

public interface IGuard
{
	/// <summary>
	/// Checks if the given claim allows the given scope or any prefix with the '*' wildcard.
	/// </summary>
	/// <param name="claim">A scope claim (i.e. a string that contains a list of scopes separated by ' ').</param>
	/// <param name="scope">The scope to check.</param>
	/// <returns>True if the claim allows the scope or a wildcard prefix, false otherwise.</returns>
	bool Allows(ScopesClaim claim, Scope scope);
}
