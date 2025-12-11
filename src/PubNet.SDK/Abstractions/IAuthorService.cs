using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

public interface IAuthorService
{
	Task<AuthorsResponse?> GetAuthorsAsync(CancellationToken cancellationToken = default);

	Task<AuthorDto?> GetAuthorAsync(string username, CancellationToken cancellationToken = default);

	Task UpdateAuthorAsync(string username, EditAuthorRequest request, CancellationToken cancellationToken = default);

	Task DeleteAuthorAsync(string username, DeleteAuthorRequest request, CancellationToken cancellationToken = default);
}
