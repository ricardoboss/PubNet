using PubNet.Database.Entities;

namespace PubNet.API.Abstractions.CQRS.Commands;

public interface IAuthorDmo
{
	Task<Author> CreateAuthorAsync(string userName, CancellationToken cancellationToken = default);
}
