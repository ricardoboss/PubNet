﻿using Microsoft.EntityFrameworkCore;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.Database.Context;
using PubNet.Database.Entities.Nuget;

namespace PubNet.API.Services.CQRS.Queries.Packages;

public class NugetPackageDao(PubNetContext context) : INugetPackageDao
{
	public Task<NugetPackage?> TryGetByPackageIdAsync(string packageId, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(context.NugetPackages.SingleOrDefault(p => p.Name == packageId));
	}

	public async Task<NugetPackageListDto> SearchAsync(string? q = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
	{
		var query = context.NugetPackages.AsQueryable();

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
			Packages = packages.Select(p => NugetPackageDto.MapFrom(p)),
		};
	}
}
