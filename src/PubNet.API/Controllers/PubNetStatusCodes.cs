namespace PubNet.API.Controllers;

public static class PubNetStatusCodes
{
	public const int Status460EmailNotFound = 460;
	public const int Status461InvalidPassword = 461;

	public static string? ToErrorCode(int code) => code switch
	{
		Status461InvalidPassword => "invalid-password",
		Status460EmailNotFound => "email-not-found",
		_ => null,
	};

	public static string? ToErrorMessage(int code) => code switch
	{
		Status461InvalidPassword => "Invalid password",
		Status460EmailNotFound => "E-mail address not found",
		_ => null,
	};
}
