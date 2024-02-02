namespace PubNet.API.Abstractions.CQRS.Commands.Packages;

public interface IDartPackageDmo
{
	Task RetractAsync(string name, string version, CancellationToken cancellationToken = default);

	Task DiscontinueAsync(string name, CancellationToken cancellationToken = default);
}
