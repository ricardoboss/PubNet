using PubNet.API.DTO.Authors;
using PubNet.Database.Entities;

namespace PubNet.API.Abstractions.CQRS.Queries;

public interface IAuthorDao
{
	Task<Author?> TryFindByUsernameAsync(string userName, CancellationToken cancellationToken = default);

	Task<AuthorListDto> SearchAsync(string? q = null, int? skip = null, int? take = null,
		CancellationToken cancellationToken = default);
}
