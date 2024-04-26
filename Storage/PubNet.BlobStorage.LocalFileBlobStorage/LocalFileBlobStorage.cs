using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.Abstractions.Args;

namespace PubNet.BlobStorage.LocalFileBlobStorage;

public class LocalFileBlobStorage(IConfiguration configuration) : IBlobStorage
{
	public const string ServiceKey = nameof(LocalFileBlobStorage);

	/// <inheritdoc />
	public string Name => configuration["Name"] ?? ServiceKey;

	private string RootPath => configuration["RootPath"] ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PubNet.BlobStorage.LocalFileBlobStorage");

	private string GetBucketPath(string bucketName)
	{
		if (Path.GetInvalidPathChars().Any(bucketName.Contains))
			throw new InvalidBucketNameException($"The bucket name '{bucketName}' contains invalid characters.");

		return Path.Combine(RootPath, bucketName);
	}

	private string GetBlobPath(string bucketName, string blobName)
	{
		if (Path.GetInvalidPathChars().Any(blobName.Contains))
			throw new InvalidBlobNameException($"The blob name '{blobName}' contains invalid characters.");

		return Path.Combine(GetBucketPath(bucketName), blobName);
	}

	private string GetBlobContentPath(string bucketName, string blobName) => Path.Combine(GetBlobPath(bucketName, blobName), "content.bin");

	private string GetBlobMetadataPath(string bucketName, string blobName) => Path.Combine(GetBlobPath(bucketName, blobName), "metadata.json");

	/// <inheritdoc />
	public Task<bool> BucketExistsAsync(IBucketExistsArgs args, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var bucketPath = GetBucketPath(args.BucketName);

		return Task.FromResult(Directory.Exists(bucketPath));
	}

	/// <inheritdoc />
	public Task<bool> CreateBucketAsync(ICreateBucketArgs args, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var bucketPath = GetBucketPath(args.BucketName);

		if (Directory.Exists(bucketPath))
			return Task.FromResult(false);

		Directory.CreateDirectory(bucketPath);

		return Task.FromResult(true);
	}

	/// <inheritdoc />
	public Task<bool> DeleteBucketAsync(IDeleteBucketArgs args, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var bucketPath = GetBucketPath(args.BucketName);

		if (!Directory.Exists(bucketPath))
			return Task.FromResult(false);

		Directory.Delete(bucketPath, true);

		return Task.FromResult(true);
	}

	/// <inheritdoc />
	public Task<IBucketItem> GetBucketAsync(IGetBucketArgs args, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var bucketPath = GetBucketPath(args.BucketName);

		var directoryInfo = new DirectoryInfo(bucketPath);

		return Task.FromResult<IBucketItem>(new DirectoryBucketItem(directoryInfo));
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<IBucketItem> ListBucketsAsync(IListBucketsArgs args, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		foreach (var directoryInfo in new DirectoryInfo(RootPath).EnumerateDirectories())
		{
			yield return await Task.FromResult(new DirectoryBucketItem(directoryInfo));
		}
	}

	/// <inheritdoc />
	public Task<bool> BlobExistsAsync(IBlobExistsArgs args, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var blobPath = GetBlobPath(args.BucketName, args.BlobName);

		return Task.FromResult(File.Exists(blobPath));
	}

	/// <inheritdoc />
	public async Task<string> PutBlobAsync(IPutBlobArgs args, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var blobContentPath = GetBlobContentPath(args.BucketName, args.BlobName);

		Directory.CreateDirectory(Path.GetDirectoryName(blobContentPath)!);

		string contentSha256;
		await using (var contentStream = await args.GetContentStreamAsync(cancellationToken))
		{
			var seekableStream = contentStream;
			if (!contentStream.CanSeek)
			{
				seekableStream = new MemoryStream();
				await contentStream.CopyToAsync(seekableStream, cancellationToken);
				seekableStream.Seek(0, SeekOrigin.Begin);
			}

			await using (var fileStream = File.Create(blobContentPath))
				await seekableStream.CopyToAsync(fileStream, cancellationToken);

			seekableStream.Seek(0, SeekOrigin.Begin);

			using var sha = SHA256.Create();
			var hash = await sha.ComputeHashAsync(seekableStream, cancellationToken);
			contentSha256 = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
		}

		var blobMetadataPath = GetBlobMetadataPath(args.BucketName, args.BlobName);

		var metadata = new FileBlobMetadata(args.ContentType, contentSha256);

		var metadataJson = JsonSerializer.Serialize(metadata);
		var metadataBytes = Encoding.UTF8.GetBytes(metadataJson);
		var metadataStream = new MemoryStream(metadataBytes);

		await using (var fileStream = File.Create(blobMetadataPath))
			await metadataStream.CopyToAsync(fileStream, cancellationToken);

		return contentSha256;
	}

	/// <inheritdoc />
	public Task<bool> DeleteBlobAsync(IDeleteBlobArgs args, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var blobPath = GetBlobPath(args.BucketName, args.BlobName);

		if (!Directory.Exists(blobPath))
			return Task.FromResult(false);

		Directory.Delete(blobPath, recursive: true);

		return Task.FromResult(true);
	}

	/// <inheritdoc />
	public async Task<IBlobItem> GetBlobAsync(IGetBlobArgs args, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var blobContentPath = GetBlobContentPath(args.BucketName, args.BlobName);
		var blobMetadataPath = GetBlobMetadataPath(args.BucketName, args.BlobName);

		var blobMetadata = await File.ReadAllTextAsync(blobMetadataPath, cancellationToken);

		var metadata = JsonSerializer.Deserialize<FileBlobMetadata>(blobMetadata);
		if (metadata is null)
			throw new BlobMetadataDeserializationFailureException("The deserialized metadata was null.", args.BucketName, args.BlobName);

		return new FileBlobItem(args.BucketName, args.BlobName, metadata.ContentType, metadata.ContentSha256, new(blobContentPath));
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<IBlobItem> ListBlobsAsync(IListBlobsArgs args, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		foreach (var fileInfo in new DirectoryInfo(GetBucketPath(args.BucketName)).EnumerateFiles())
		{
			var blobMetadataPath = GetBlobMetadataPath(args.BucketName, fileInfo.Name);

			var blobMetadata = await File.ReadAllTextAsync(blobMetadataPath, cancellationToken);

			var metadata = JsonSerializer.Deserialize<FileBlobMetadata>(blobMetadata);
			if (metadata is null)
				throw new BlobMetadataDeserializationFailureException("The deserialized metadata was null.", args.BucketName, fileInfo.Name);

			yield return new FileBlobItem(args.BucketName, fileInfo.Name, metadata.ContentType, metadata.ContentSha256, fileInfo);
		}
	}
}
