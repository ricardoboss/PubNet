using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record ListBlobsArgs(string BucketName, string? Pattern = null, string? ContentType = null) : IListBlobsArgs;
