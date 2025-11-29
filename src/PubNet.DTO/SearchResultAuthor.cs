using JetBrains.Annotations;

namespace PubNet.API.DTO;

[PublicAPI]
public record SearchResultAuthor(string UserName, string Name, int NumPackagesUploaded, DateTimeOffset Joined);
