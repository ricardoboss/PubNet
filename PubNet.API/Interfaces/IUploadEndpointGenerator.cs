using PubNet.API.DTO;
using PubNet.Models;

namespace PubNet.API.Interfaces;

public interface IUploadEndpointGenerator
{
    public Task<UploadEndpointData> GenerateUploadEndpointData(HttpRequest request, Author author, CancellationToken cancellationToken = default);
}