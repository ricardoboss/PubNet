using PubNet.API.DTO;
using PubNet.API.DTO.Packages;
using PubNet.Database.Models;

namespace PubNet.API.Interfaces;

public interface IUploadEndpointGenerator
{
	public Task<UploadEndpointDataDto> GenerateUploadEndpointData(HttpRequest request, Author author,
		CancellationToken cancellationToken = default);
}
