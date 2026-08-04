namespace PubNet.API.DTO.Authentication.Errors;

public class RegistrationsDisabledErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public static string DefaultMessage => "Registrations are disabled";
}
