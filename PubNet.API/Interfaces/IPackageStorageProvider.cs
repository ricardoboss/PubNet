using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Interfaces;

public interface IPackageStorageProvider
{
    public Task<string> StoreArchive(string name, string version, Stream stream);

    public FileResult ReadArchive(string name, string version);
}