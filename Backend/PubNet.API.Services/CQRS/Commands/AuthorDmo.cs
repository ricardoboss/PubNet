using PubNet.API.Abstractions.CQRS.Commands;
using PubNet.Database.Context;
using PubNet.Database.Entities;

namespace PubNet.API.Services.CQRS.Commands;

public class AuthorDmo(PubNetContext context) : IAuthorDmo
{
	public async Task<Author> CreateAuthorAsync(string userName, CancellationToken cancellationToken = default)
	{
		// TODO: validate user name

		var author = new Author
		{
			UserName = userName,
			RegisteredAtUtc = DateTimeOffset.UtcNow,
		};

		await context.Authors.AddAsync(author, cancellationToken);

		await context.SaveChangesAsync(cancellationToken);

		await context.Entry(author).ReloadAsync(cancellationToken);

		return author;
	}
}
