using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication.Errors;

[PublicAPI]
public class UsernameAlreadyInUseErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public string DefaultMessage => "Username already in use";
}
