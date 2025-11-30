using Microsoft.EntityFrameworkCore;
using PubNet.Common.Interfaces;
using PubNet.Common.Models;
using PubNet.Common.Services;
using PubNet.Database;
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
			services.AddDbContext<PubNetContext>(
				options => options
					.UseNpgsql(context.Configuration.GetConnectionString("PubNet"))
			);

			// for used to analyze uploaded packages
			services.AddSingleton<DartCli>();

			// manages tasks for the worker
			services.AddSingleton<WorkerTaskQueue>();

			// for managing packages stored on the local host
			services.AddPackageStorageProviderOptions();
			services.AddSingleton<IPackageStorageProvider, LocalPackageStorageProvider>();

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
