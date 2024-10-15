using PubNet.Database.Entities;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.CQRS.Commands;

public interface IIdentityDmo
{
	Task<Identity> CreateIdentityAsync(Author author, string email, string password, CancellationToken cancellationToken = default);
}
