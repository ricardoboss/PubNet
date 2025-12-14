namespace PubNet.API.DTO.Authentication.Errors;

public class RegistrationsDisabledErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public string DefaultMessage => "Registrations are disabled";
}
