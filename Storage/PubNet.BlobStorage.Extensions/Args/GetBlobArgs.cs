using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record GetBlobArgs(string BucketName, string BlobName) : IGetBlobArgs;
