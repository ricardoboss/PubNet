namespace PubNet.API.Abstractions;

public interface ISecureTokenGenerator
{
	string GenerateSecureToken(int length);
}
