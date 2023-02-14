using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO;

public class EditAuthorRequest
{
	[Required]
	public string? Name { get; set; }

	[DataType(DataType.Url)]
	public string? Website { get; set; }
}
