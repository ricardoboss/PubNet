using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages;

[PublicAPI]
public record UploadEndpointDataDto(string Url, Dictionary<string, string> Fields);
