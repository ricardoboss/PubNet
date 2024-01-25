using PubNet.Database.Entities.Auth;
using PubNet.Web.Models;

namespace PubNet.API.Abstractions.Authentication;

public interface IJwtFactory
{
	JsonWebToken Create(Token token);
}
