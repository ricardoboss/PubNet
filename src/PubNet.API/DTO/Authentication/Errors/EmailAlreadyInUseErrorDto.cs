using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication.Errors;

[PublicAPI]
public class EmailAlreadyInUseErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public static string DefaultMessage => "E-mail address already in use";
}
