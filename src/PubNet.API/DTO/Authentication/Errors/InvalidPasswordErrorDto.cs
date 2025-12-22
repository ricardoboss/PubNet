using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication.Errors;

[PublicAPI]
public class InvalidPasswordErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public string DefaultMessage => "Invalid password";
}
