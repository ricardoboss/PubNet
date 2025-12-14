using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages.Errors;

[PublicAPI]
public class PackageDiscontinuedErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public string DefaultMessage => "Package discontinued";
}
