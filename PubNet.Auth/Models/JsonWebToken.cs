using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vogen;

namespace PubNet.Auth.Models;

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

	private static string NormalizeInput(string input) => input.Trim();

	public IEnumerable<Claim> EnumerateClaims()
	{
		var parts = Value.Split('.', 3, StringSplitOptions.RemoveEmptyEntries);
		if (parts is not [_, var claimsEncoded, _])
			throw new FormatException("JWT has invalid format.");

		var claimsPadded = claimsEncoded.PadRight(claimsEncoded.Length + (4 - claimsEncoded.Length % 4) % 4, '=');

		var claimsDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(claimsPadded));
		var claims =
			JsonSerializer.Deserialize(claimsDecoded, JsonWebTokenSerializerContext.Default.DictionaryStringObject);
		if (claims is null)
			throw new FormatException("JWT claims are invalid.");

		return claims.Select(c => new Claim(c.Key, c.Value.ToString() ?? string.Empty));
	}
}

[JsonSerializable(typeof(Dictionary<string, object>))]
public partial class JsonWebTokenSerializerContext : JsonSerializerContext;
