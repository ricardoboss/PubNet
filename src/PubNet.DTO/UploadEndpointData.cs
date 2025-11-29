using JetBrains.Annotations;

namespace PubNet.API.DTO;

[PublicAPI]
public record UploadEndpointData(string Url, Dictionary<string, string> Fields);
