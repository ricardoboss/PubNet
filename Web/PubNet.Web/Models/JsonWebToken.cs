using Vogen;

namespace PubNet.Web.Models;

[ValueObject<string>]
public readonly partial struct JsonWebToken
{
	private static Validation Validate(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return Validation.Invalid("The given token is empty.");

		if (input.Length > 1024)
			return Validation.Invalid("The given token is too long.");

		if (input.Length < 64)
			return Validation.Invalid("The given token is too short.");

		if (!input.Contains('.'))
			return Validation.Invalid("The given token is not a valid JWT (missing dots).");

		if (input.Split('.').Length != 3)
			return Validation.Invalid("The given token is not a valid JWT (too many dots).");

		if (input.Split('.').Any(string.IsNullOrWhiteSpace))
			return Validation.Invalid("The given token is not a valid JWT (empty segments).");

		return Validation.Ok;
	}
}
