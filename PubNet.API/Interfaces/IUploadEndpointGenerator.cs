using PubNet.API.DTO;
using PubNet.API.Models;

namespace PubNet.API.Interfaces;

public interface IUploadEndpointGenerator
{
    public Task<UploadEndpointData> GenerateUploadEndpointData(HttpRequest request, AuthorToken authorToken);
}