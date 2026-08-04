using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages.Errors;

[PublicAPI]
public class PackageDiscontinuedErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public static string DefaultMessage => "Package discontinued";
}
