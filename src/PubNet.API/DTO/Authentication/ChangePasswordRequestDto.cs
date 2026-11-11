namespace PubNet.API.DTO.Authentication;

public class ChangePasswordRequestDto
{
	public string OldPassword { get; set; } = string.Empty;
	public string NewPassword { get; set; } = string.Empty;
}