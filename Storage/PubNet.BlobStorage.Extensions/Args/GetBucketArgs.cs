using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record GetBucketArgs(string BucketName) : IGetBucketArgs;
