using PubNet.Database.Entities;

namespace PubNet.API.Abstractions.CQRS.Queries;

public interface IAuthorDao
{
	Task<Author?> TryFindByUsernameAsync(string userName, CancellationToken cancellationToken = default);
}
