using PubNet.API.Abstractions.CQRS.Commands.Packages;

namespace PubNet.API.Services.CQRS.Commands.Packages;

public class DartPackageDmo : IDartPackageDmo
{
	public async Task RetractAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task DiscontinueAsync(string name, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
