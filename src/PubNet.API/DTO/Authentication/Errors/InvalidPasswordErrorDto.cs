using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication.Errors;

[PublicAPI]
public class InvalidPasswordErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public static string DefaultMessage => "Invalid password";
}
