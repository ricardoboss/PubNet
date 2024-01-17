using Microsoft.EntityFrameworkCore;
using Npgsql;
using PubNet.ArchiveStorage.BlobStorage;
using PubNet.BlobStorage.Abstractions;
using PubNet.BlobStorage.LocalFileBlobStorage;
using PubNet.Database;
using PubNet.Database.Context;
using PubNet.DocsStorage.Abstractions;
using PubNet.DocsStorage.LocalFileDocsStorage;
using PubNet.PackageStorage.Abstractions;
using PubNet.Worker;
using PubNet.Worker.Services;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	.Enrich.FromLogContext()
	.CreateBootstrapLogger();

try
{
	var builder = Host.CreateDefaultBuilder(args)
		.UseSerilog((context, services, configuration) =>
			configuration.ReadFrom.Configuration(context.Configuration)
				.ReadFrom.Services(services)
				.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
				.Enrich.FromLogContext()
				.WriteTo.Console()
		)
		.ConfigureServices((context, services) =>
		{
#pragma warning disable CS0618 // Type or member is obsolete
			NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
#pragma warning restore CS0618 // Type or member is obsolete

			services.AddDbContext<PubNetContext>(
				options => options
					.UseNpgsql(context.Configuration.GetConnectionString("PubNet"))
			);

			// for used to analyze uploaded packages
			services.AddSingleton<DartCli>();

			// manages tasks for the worker
			services.AddSingleton<WorkerTaskQueue>();

			// for managing packages stored on the local host
			services.AddSingleton<IBlobStorage, LocalFileBlobStorage>();
			services.AddSingleton<IArchiveStorage, BlobArchiveStorage>();
			services.AddSingleton<IDocsStorage, LocalFileDocsStorage>();

			services.AddHostedService<Worker>();
		});

	var app = builder.Build();

	await PubNetContext.RunMigrations(app.Services);

	await app.RunAsync();
}
catch (Exception e)
{
	Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}
