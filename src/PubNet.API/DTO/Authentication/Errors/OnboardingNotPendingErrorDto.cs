namespace PubNet.API.DTO.Authentication.Errors;

public class OnboardingNotPendingErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public string DefaultMessage => "This instance has already been set up";
}
