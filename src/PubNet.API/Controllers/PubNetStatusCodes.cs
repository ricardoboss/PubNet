namespace PubNet.API.Controllers;

public static class PubNetStatusCodes
{
	// Mapped Status Codes
	public const int Status400BadRequest = 400;
	public const int Status401Unauthenticated = 401;
	public const int Status403Forbidden = 403;
	public const int Status404NotFound = 404;
	public const int Status411LengthRequired = 411;
	public const int Status413PayloadTooLarge = 413;
	public const int Status500InternalServerError = 500;

	// Authentication (46x)
	public const int Status460EmailNotFound = 460;
	public const int Status461InvalidPassword = 461;
	public const int Status462RegistrationsDisabled = 462;
	public const int Status463UsernameAlreadyInUse = 463;
	public const int Status464EmailAlreadyInUse = 464;

	// Package Upload (47x)
	public const int Status470MissingRequiredData = 470;
	public const int Status471InvalidUploadData = 471;
	public const int Status472InvalidPubSpec = 472;
	public const int Status473PackageDiscontinued = 473;
	public const int Status474VersionConflict = 474;

	public static string? ToErrorCode(int code) => code switch
	{
		Status460EmailNotFound => "email-not-found",
		Status461InvalidPassword => "invalid-password",
		_ => null,
	};

	public static string? ToErrorMessage(int code) => code switch
	{
		// Status401Unauthenticated => "Unauthenticated",

		Status460EmailNotFound => "E-mail address not found",
		Status461InvalidPassword => "Invalid password",
		Status462RegistrationsDisabled => "Registrations are disabled",
		Status463UsernameAlreadyInUse => "Username already in use",
		Status464EmailAlreadyInUse => "E-mail address already in use",

		Status470MissingRequiredData => "Missing required data",
		Status471InvalidUploadData => "Invalid upload data",
		Status472InvalidPubSpec => "Invalid pubspec.yaml",
		Status473PackageDiscontinued => "Package discontinued",
		Status474VersionConflict => "Version conflict",

		_ => null,
	};
}
