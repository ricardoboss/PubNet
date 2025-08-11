using System.Text.Json.Serialization;

namespace PubNet.BlobStorage.LocalFileBlobStorage;

public record FileBlobMetadata(string ContentType, string ContentSha256);

[JsonSerializable(typeof(FileBlobMetadata))]
public partial class FileBlobMetadataJsonSerializerContext : JsonSerializerContext;
