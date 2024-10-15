using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.Extensions.Args;

internal record ListBucketsArgs(string? Pattern = null) : IListBucketsArgs;
