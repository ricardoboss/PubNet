namespace PubNet.API.Mails;

public class WelcomeMailModel
{
	public required string UserName { get; init; }

	public required Uri FrontendUrl { get; init; }
}
