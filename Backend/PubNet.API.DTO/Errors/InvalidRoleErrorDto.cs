namespace PubNet.API.DTO.Errors;

public class InvalidRoleErrorDto
{
	public required string? RequiredRole { get; init; }

	public required string? ClaimedRole { get; init; }

	public string? Message { get; init; }
}
