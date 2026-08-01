namespace PubNet.API.Mails;

public class SetupCompletedMailModel
{
	public required string UserName { get; init; }

	public required Uri FrontendUrl { get; init; }
}
