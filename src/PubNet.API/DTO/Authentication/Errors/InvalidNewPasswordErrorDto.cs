using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication.Errors;

[PublicAPI]
public class InvalidNewPasswordErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public static string DefaultMessage => "Invalid new password";
}
