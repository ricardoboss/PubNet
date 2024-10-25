using System.Text.RegularExpressions;
using Vogen;

namespace PubNet.Auth.Models;

[ValueObject<string>]
[Instance("Empty", "")]
public readonly partial struct ScopesClaim
{
	[GeneratedRegex(@"^[a-z:\s\*]*$")]
	private static partial Regex ValidScopesClaimRegex();

	private static Validation Validate(string input)
	{
		return !ValidScopesClaimRegex().IsMatch(input)
			? Validation.Invalid("Only lowercase letters, colons, and spaces are allowed.")
			: Validation.Ok;
	}

	private static string NormalizeInput(string input) => input.Trim().ToLowerInvariant();

	public IEnumerable<Scope> ToScopes() => Value.Split(JwtClaims.ScopeSeparator).Select(Scope.From);
}
