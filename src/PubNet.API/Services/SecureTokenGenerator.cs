using System.Security.Cryptography;
using System.Text;

namespace PubNet.API.Services;

/// <summary>
/// Creates and hashes opaque, URL-safe secrets, like password reset tokens. Generation and hashing live
/// together so every flow handing out such a secret stores it the same way: only ever the hash, never the
/// secret itself.
/// </summary>
public class SecureTokenGenerator
{
	private const int TokenSizeInBytes = 32;

	public string GenerateToken() => Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(TokenSizeInBytes));

	public string HashToken(string token) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
