using Microsoft.Extensions.Logging;
using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class CachingAuthorService(IAuthorService inner, ILogger<CachingAuthorService> logger) : IAuthorService
{
	private readonly Dictionary<string, AuthorDto> cache = new();
	private AuthorsResponseDto? authors;

	public async Task<AuthorsResponseDto?> GetAuthorsAsync(CancellationToken cancellationToken = default)
	{
		if (authors is not null)
			return authors;

		return authors = await inner.GetAuthorsAsync(cancellationToken);
	}

	public async Task<AuthorDto?> GetAuthorAsync(string username, CancellationToken cancellationToken = default)
	{
		if (cache.TryGetValue(username, out var author))
		{
			logger.LogTrace("Providing cached author {{ username={AuthorUsername} }}", username);

			return author;
		}

		logger.LogTrace("Cache miss for author {{ username={AuthorUsername} }}", username);

		author = await inner.GetAuthorAsync(username, cancellationToken);

		if (author is not null)
		{
			logger.LogTrace("Caching author {{ username={AuthorUsername} }}", username);

			cache[username] = author;
		}

		return author;
	}

	public async Task UpdateAuthorAsync(string username, EditAuthorRequestDto request, CancellationToken cancellationToken = default)
	{
		await inner.UpdateAuthorAsync(username, request, cancellationToken);

		if (!cache.ContainsKey(username))
			return;

		logger.LogTrace("Invalidating cache for author {{ username={AuthorUsername} }}", username);

		cache.Remove(username);
	}

	public async Task DeleteAuthorAsync(string username, DeleteAuthorRequestDto request, CancellationToken cancellationToken = default)
	{
		await inner.DeleteAuthorAsync(username, request, cancellationToken);

		if (!cache.ContainsKey(username))
			return;

		logger.LogTrace("Invalidating cache for author {{ username={AuthorUsername} }}", username);

		cache.Remove(username);
	}
}
