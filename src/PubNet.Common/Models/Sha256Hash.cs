using Vogen;

namespace PubNet.Common.Models;

[ValueObject<string>]
public readonly partial struct Sha256Hash
{
	private static Validation Validate(string input)
	{
		if (input.Length != 64)
			return Validation.Invalid("SHA256 hashes need to be 64 characters long (hex encoded)");

		if (!input.All(char.IsAsciiHexDigitUpper))
			return Validation.Invalid("SHA256 hashes can only contain uppercase ASCII hex digits");

		return Validation.Ok;
	}

	private static string NormalizeInput(string input) => input.ToUpperInvariant();
}
