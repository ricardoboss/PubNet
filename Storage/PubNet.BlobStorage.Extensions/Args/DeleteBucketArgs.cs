using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record DeleteBucketArgs(string BucketName) : IDeleteBucketArgs;
