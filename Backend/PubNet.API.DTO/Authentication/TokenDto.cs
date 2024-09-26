using System.ComponentModel.DataAnnotations;
using PubNet.Database.Entities.Auth;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Authentication;

[Mapper]
public partial class TokenDto
{
	public static partial TokenDto MapFrom(Token token);

	[Required]
	public required Guid Id { get; init; }

	[Required]
	public required string Name { get; init; }

	[Required]
	public required string IpAddress { get; init; }

	[Required]
	public required string UserAgent { get; init; }

	[Required]
	public required string DeviceType { get; init; }

	[Required]
	public required string Browser { get; init; }

	[Required]
	public required string Platform { get; init; }

	[Required]
	public required IEnumerable<string> Scopes { get; init; }

	[Required, DataType(DataType.DateTime)]
	public required DateTimeOffset CreatedAtUtc { get; init; }

	[Required, DataType(DataType.DateTime)]
	public required DateTimeOffset ExpiresAtUtc { get; init; }

	[Required, DataType(DataType.DateTime)]
	public required DateTimeOffset? RevokedAtUtc { get; init; }
}
