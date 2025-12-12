using JetBrains.Annotations;

namespace PubNet.API.DTO.Authors;

[PublicAPI]
public record SearchResultAuthorDto(string UserName, string Name, int NumPackagesUploaded, DateTimeOffset Joined);
