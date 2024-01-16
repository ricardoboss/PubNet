using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PubNet.Database.Context;

namespace PubNet.Database.Design;

public class DesignTimePubNetContextFactory : IDesignTimeDbContextFactory<PubNetContext>
{
	/// <inheritdoc />
	public PubNetContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<PubNetContext>();

		// TODO: consider using IConfiguration to get the connection string
		optionsBuilder
			.UseNpgsql("Host=localhost;Database=pubnet;Username=pubnet;Password=pubnet")
			// .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
			;

		return new(optionsBuilder.Options);
	}
}
