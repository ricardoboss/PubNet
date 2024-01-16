using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record DeleteBlobArgs(string BucketName, string BlobName) : IDeleteBlobArgs;
