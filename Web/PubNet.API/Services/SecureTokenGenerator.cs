using System.Security.Cryptography;
using PubNet.API.Abstractions;

namespace PubNet.API.Services;

public class SecureTokenGenerator : ISecureTokenGenerator
{
	public string GenerateSecureToken(int length)
	{
		const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

		return RandomNumberGenerator.GetString(validChars, length);
	}
}
