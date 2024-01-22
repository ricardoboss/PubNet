using Microsoft.EntityFrameworkCore;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.Database.Context;
using PubNet.Database.Entities;

namespace PubNet.API.Services.CQRS.Queries;

public class AuthorDao(PubNetContext context) : IAuthorDao
{
	public Task<Author?> TryFindByUsernameAsync(string userName, CancellationToken cancellationToken = default)
	{
		return context.Authors.FirstOrDefaultAsync(a => a.UserName == userName, cancellationToken);
	}
}
