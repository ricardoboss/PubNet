namespace PubNet.API.DTO.Authors.Errors;

public class LastAdminErrorDto : ErrorMessageDto, IHaveDefaultMessage
{
	public string DefaultMessage =>
		"You are the only administrator of this instance. Make someone else an administrator before deleting your account";
}
