using Microsoft.AspNetCore.Http;

namespace PubNet.API.Abstractions.Authentication;

public class EmailAlreadyExistsException(string email) : ApiException("email-address-already-taken",
	$"E-mail address {email} already exists", StatusCodes.Status409Conflict);
