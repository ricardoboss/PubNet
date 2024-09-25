using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Authentication;

public class CreatePersonalAccessTokenDto
{
	[Required]
	public required string Name { get; init; }

	[Required]
	public IEnumerable<string> Scopes { get; init; } = [];

	[Required, DataType(DataType.Duration)]
	public required int LifetimeInDays { get; init; }
}
