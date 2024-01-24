using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Authentication;

public interface IJwtFactory
{
	string Create(Token token);
}
