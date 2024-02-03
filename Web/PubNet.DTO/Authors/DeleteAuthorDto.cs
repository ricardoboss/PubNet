namespace PubNet.API.DTO.Authors;

public class DeleteAuthorDto
{
	// Username of author to delete is supplied via route parameter

	/// <summary>
	/// This is the password of the authenticated user, which is not necessarily the same person as the author being
	/// deleted.
	/// </summary>
	public required string Password { get; init; }
}
