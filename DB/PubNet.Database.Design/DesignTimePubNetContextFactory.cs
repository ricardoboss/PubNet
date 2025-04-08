using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PubNet.Database.Context;

namespace PubNet.Database.Design;

public class DesignTimePubNetContextFactory : IDesignTimeDbContextFactory<PubNet2Context>
{
	/// <inheritdoc />
	public PubNet2Context CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<PubNet2Context>();

		// TODO: consider using IConfiguration to get the connection string
		optionsBuilder
			.UseNpgsql("Host=localhost;Database=pubnet2;Username=pubnet2;Password=pubnet2")
			// .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
			;

		return new(optionsBuilder.Options);
	}
}
