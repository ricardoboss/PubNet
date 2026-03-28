using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

internal sealed class ApiAuthorService(PubNetApiClient apiClient) : IAuthorService
{
	public async Task<AuthorsResponseDto?> GetAuthorsAsync(CancellationToken cancellationToken = default)
	{
		return await apiClient.Authors.GetAsync(cancellationToken: cancellationToken);
	}

	public async Task<AuthorDto?> GetAuthorAsync(string username, CancellationToken cancellationToken = default)
	{
		return await apiClient.Authors[username].GetAsync(cancellationToken: cancellationToken);
	}

	public async Task UpdateAuthorAsync(string username, EditAuthorRequestDto request, CancellationToken cancellationToken = default)
	{
		await apiClient.Authors[username].PatchAsync(request, cancellationToken: cancellationToken);
	}

	public async Task DeleteAuthorAsync(string username, DeleteAuthorRequestDto request, CancellationToken cancellationToken = default)
	{
		await apiClient.Authors[username].DeletePath.PostAsync(request, cancellationToken: cancellationToken);
	}
}
