namespace PubNet.Client.Extensions;

public interface IPackageQueryBuilder<TPackage>
{
	public IPackageQueryBuilder<TPackage> WithVersion(string version);

	public Task<TPackage> RunAsync(CancellationToken? cancellationToken = default);
}
