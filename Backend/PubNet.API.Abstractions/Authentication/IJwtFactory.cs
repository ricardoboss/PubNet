using PubNet.Database.Entities.Auth;
using PubNet.Auth.Models;

namespace PubNet.API.Abstractions.Authentication;

public interface IJwtFactory
{
	JsonWebToken Create(Token token);
}
