using Microsoft.EntityFrameworkCore;
using PubNet.API.Abstractions.CQRS.Queries.Packages;
using PubNet.Database.Context;
using PubNet.Database.Entities.Dart;

namespace PubNet.API.Services.CQRS.Queries.Packages;

public class DartPackageDao(PubNetContext context) : IDartPackageDao
{
	public async Task<DartPackage?> TryGetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return await context.DartPackages.SingleOrDefaultAsync(p => p.Name == name, cancellationToken);
	}
}
