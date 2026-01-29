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
		Status462RegistrationsDisabled => "registrations-disabled",
		Status463UsernameAlreadyInUse => "username-in-use",
		Status464EmailAlreadyInUse => "email-in-use",

		Status470MissingRequiredData => "missing-required-data",
		Status471InvalidUploadData => "invalid-upload-data",
		Status472InvalidPubSpec => "invalid-pubspec",
		Status473PackageDiscontinued => "package-discontinued",
		Status474VersionConflict => "version-conflict",

		_ => null,
	};
}
