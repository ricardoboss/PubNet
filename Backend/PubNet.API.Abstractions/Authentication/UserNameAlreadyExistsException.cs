using Microsoft.AspNetCore.Http;

namespace PubNet.API.Abstractions.Authentication;

public class UserNameAlreadyExistsException(string userName)
	: ApiException("username-already-taken", $"User name '{userName}' already exists.", StatusCodes.Status409Conflict);
