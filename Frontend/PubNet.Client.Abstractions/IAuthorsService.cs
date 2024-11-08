using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Abstractions;

public interface IAuthorsService
{
	Task<AuthorDto?> GetAuthorAsync(string name, CancellationToken cancellationToken = default);

	Task<AuthorListDto?> GetAuthorsAsync(string? query = null, int? page = null, int? perPage = null,
		CancellationToken? cancellationToken = default);
}
