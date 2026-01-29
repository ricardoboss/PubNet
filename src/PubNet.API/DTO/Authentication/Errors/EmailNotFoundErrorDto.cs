using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication.Errors;

[PublicAPI]
public class EmailNotFoundErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public string DefaultMessage => "E-mail address not found";
}
