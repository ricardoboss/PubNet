using PubNet.API.DTO;
using PubNet.Database.Entities;

namespace PubNet.API.Interfaces;

public interface IUploadEndpointGenerator
{
	public Task<UploadEndpointData> GenerateUploadEndpointData(HttpRequest request, Author author,
		CancellationToken cancellationToken = default);
}
