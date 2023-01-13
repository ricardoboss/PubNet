namespace PubNet.API.Interfaces;

public interface IPackageStorageProvider
{
    public Task<string> StoreArchive(string name, string version, Stream stream);

    public Stream ReadArchive(string name, string version);
}