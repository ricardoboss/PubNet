using PubNet.SDK.Abstractions;
using PubNet.SDK.Generated;
using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Services;

public class ApiAuthorService(PubNetApiClient apiClient) : IAuthorService
{
	public Task<AuthorsResponse> GetAuthorsAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public Task<AuthorDto?> GetAuthorAsync(string username, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task UpdateAuthorAsync(string username, EditAuthorRequest request, CancellationToken cancellationToken = default)
	{
		await apiClient.Authors[username].PatchAsync(request, cancellationToken: cancellationToken);
	}

	public Task DeleteAuthorAsync(string username, DeleteAuthorRequest request, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
