using JetBrains.Annotations;

namespace PubNet.API.DTO.Packages;

[PublicAPI]
public record UploadEndpointData(string Url, Dictionary<string, string> Fields);
