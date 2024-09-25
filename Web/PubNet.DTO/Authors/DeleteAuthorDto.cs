using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Authors;

/// <remarks>
/// Username of author to delete is supplied via route parameter
/// </remarks>
public class DeleteAuthorDto
{
	/// <summary>
	/// This is the password of the authenticated user, which is not necessarily the same person as the author being
	/// deleted.
	/// </summary>
	[Required, DataType(DataType.Password), PasswordPropertyText(true)]
	public required string Password { get; init; }
}
