using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PubNet.API.DTO.Authentication;

[PublicAPI]
public class ResetPasswordRequestDto
{
	public string? Token { get; set; }

	[DataType(DataType.Password)]
	public string? Password { get; set; }
}
