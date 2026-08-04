using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication.Errors;

[PublicAPI]
public class InvalidPasswordResetTokenErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public static string DefaultMessage => "The password reset link is invalid, expired or has already been used";
}
