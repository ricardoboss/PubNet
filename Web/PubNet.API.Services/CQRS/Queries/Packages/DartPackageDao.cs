using Microsoft.EntityFrameworkCore;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.Services.CQRS.Queries.Packages;

public class DartPackageDao(PubNetContext context) : IDartPackageDao
{
	public async Task<DartPackage?> TryGetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return await context.DartPackages.SingleOrDefaultAsync(p => p.Name == name, cancellationToken);
	}

	public async Task<DartPackageListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		var query = context.DartPackages.AsQueryable();

		if (!string.IsNullOrWhiteSpace(q))
			query = query.Where(p => p.Name.Contains(q));

		var total = await query.CountAsync(cancellationToken);

		if (skip.HasValue)
			query = query.Skip(skip.Value);

		if (take.HasValue)
			query = query.Take(take.Value);

		var packages = await query.ToListAsync(cancellationToken);

		return new()
		{
			TotalHits = total,
			Packages = packages.Select(p => DartPackageDto.MapFrom(p)),
		};
	}
}
