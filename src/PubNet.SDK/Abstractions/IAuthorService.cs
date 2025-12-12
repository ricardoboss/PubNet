using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

public interface IAuthorService
{
	Task<AuthorsResponseDto?> GetAuthorsAsync(CancellationToken cancellationToken = default);

	Task<AuthorDto?> GetAuthorAsync(string username, CancellationToken cancellationToken = default);

	Task UpdateAuthorAsync(string username, EditAuthorRequestDto request, CancellationToken cancellationToken = default);

	Task DeleteAuthorAsync(string username, DeleteAuthorRequestDto request, CancellationToken cancellationToken = default);
}
