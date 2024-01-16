using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record BucketExistsArgs(string BucketName) : IBucketExistsArgs;
