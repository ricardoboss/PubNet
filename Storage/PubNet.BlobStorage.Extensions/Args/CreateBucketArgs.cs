using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record CreateBucketArgs(string BucketName) : ICreateBucketArgs;
