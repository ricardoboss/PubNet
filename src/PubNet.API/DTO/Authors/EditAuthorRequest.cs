using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PubNet.API.DTO.Authors;

[PublicAPI]
public class EditAuthorRequest
{
	public string? Name { get; set; }

	[DataType(DataType.Url)]
	public string? Website { get; set; }
}
