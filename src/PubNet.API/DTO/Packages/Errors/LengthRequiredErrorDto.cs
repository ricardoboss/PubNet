using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages.Errors;

[PublicAPI]
public class LengthRequiredErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public static string DefaultMessage => "The Content-Length header is required";
}
