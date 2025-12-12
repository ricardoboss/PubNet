using JetBrains.Annotations;

namespace PubNet.API.DTO;

[PublicAPI]
public class ErrorResponseDto(ErrorResponseBodyDto? error = null)
{
	public ErrorResponseBodyDto? Error { get; } = error;

	public static ErrorResponseDto UsernameMismatch =>
		new(new("author-username-mismatch",
			"The username you are trying to access does not match the owner of the token you used"));

	public static ErrorResponseDto PackageAuthorMismatch =>
		new(new("package-author-mismatch",
			"The package you are trying to access does not belong the owner of the token you used"));

	public static ErrorResponseDto InvalidQuery =>
		new(new("invalid-query", "Query parameter 'limit' is mandatory if 'before' is given"));

	public static ErrorResponseDto InvalidPasswordConfirmation =>
		new(new("invalid-password-confirmation", "The password you supplied is not correct"));

	public static ErrorResponseDto PackageLengthRequired =>
		new(new("length-required", "The Content-Length header is required"));

	public static ErrorResponseDto MissingPackageFile =>
		new(new("missing-package-file", "The package file is missing"));

	public static ErrorResponseDto MissingFields =>
		new(new("missing-fields", "Not all fields have been forwarded"));

	public static ErrorResponseDto InvalidAuthorId =>
		new(new("invalid-author-id", "Invalid author id provided"));

	public static ErrorResponseDto InvalidSignedUrl =>
		new(new("invalid-signed-url",
			"The provided signature is invalid, indicating the url has been tempered with or was used on the wrong host"));

	public static ErrorResponseDto MissingPendingId =>
		new(new("missing-pending-id", "No pending id was provided"));

	public static ErrorResponseDto InvalidPendingId =>
		new(new("invalid-pending-id", "An invalid pending id was provided"));

	public static ErrorResponseDto MissingPubspec =>
		new(new("missing-pubspec", "The package archive is missing a pubspec.yaml"));

	public static ErrorResponseDto MissingValues =>
		new(new("missing-values", "The request you sent has missing values"));

	public static ErrorResponseDto UsernameAlreadyInUse =>
		new(new("username-already-in-use", "The username you provided is already in use"));

	public static ErrorResponseDto EmailAlreadyInUse =>
		new(new("email-already-in-use", "The e-mail address you provided is already in use"));

	public static ErrorResponseDto PackageDiscontinued =>
		new(new("package-discontinued", "The package you are trying to publish has been discontinued and new versions cannot be added to it"));

	public static ErrorResponseDto RegistrationsDisabled =>
		new(new("registrations-disabled", "Registrations are currently disabled"));

	public static ErrorResponseDto VersionAlreadyExists(string packageName, string packageVersion)
	{
		return new(new("version-already-exists", $"Version {packageVersion} of {packageName} already exists"));
	}

	public static ErrorResponseDto VersionOlderThanLatest(string latestPackageVersion)
	{
		return new(new("version-older-than-latest",
			$"The version you are trying to upload is older than the latest version ({latestPackageVersion})"));
	}

	public static ErrorResponseDto PackagePayloadTooLarge(long maxUploadSize)
	{
		return new(new("payload-too-large", $"Maximum payload size is {maxUploadSize} bytes"));
	}

	public static ErrorResponseDto InvalidPubspec(string message)
	{
		return new(new("invalid-pubspec", $"An error occurred while parsing the pubspec.yaml: {message}"));
	}

	public static ErrorResponseDto FromException(Exception e)
	{
		return new(new(e.GetType().Name, e.Message));
	}
}

[PublicAPI]
public record ErrorResponseBodyDto(string Code, string Message);
