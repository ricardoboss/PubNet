namespace PubNet.API.Mails;

public class PasswordResetMailModel
{
	public required string UserName { get; init; }

	public required Uri ResetUrl { get; init; }

	public required TimeSpan ValidFor { get; init; }
}
