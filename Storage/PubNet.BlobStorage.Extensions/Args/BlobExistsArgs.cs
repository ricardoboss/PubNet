using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record BlobExistsArgs(string BucketName, string BlobName) : IBlobExistsArgs;
