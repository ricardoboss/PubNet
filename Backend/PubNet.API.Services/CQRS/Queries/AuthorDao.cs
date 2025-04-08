using Microsoft.EntityFrameworkCore;
using PubNet.API.Abstractions.CQRS.Queries;
using PubNet.API.DTO.Authors;
using PubNet.Database.Context;
using PubNet.Database.Entities;

namespace PubNet.API.Services.CQRS.Queries;

public class AuthorDao(PubNet2Context context) : IAuthorDao
{
	public Task<Author?> TryFindByUsernameAsync(string userName, CancellationToken cancellationToken = default)
	{
		return context.Authors.FirstOrDefaultAsync(a => a.UserName == userName, cancellationToken);
	}

	public async Task<AuthorListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		var query = context.Authors.AsQueryable();

		if (!string.IsNullOrWhiteSpace(q))
			query = query.Where(a => a.UserName.Contains(q));

		var total = await query.CountAsync(cancellationToken);

		if (skip.HasValue)
			query = query.Skip(skip.Value);

		if (take.HasValue)
			query = query.Take(take.Value);

		var authors = await query.ToListAsync(cancellationToken);

		return new()
		{
			TotalHits = total,
			Authors = authors.Select(AuthorDto.MapFrom),
		};
	}
}
