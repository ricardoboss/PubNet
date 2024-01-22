using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Authentication;

public interface IJwtFactory
{
	const string TokenValueClaim = "token";
	const string ScopeClaim = "scope";

	string Create(Token token);
}
