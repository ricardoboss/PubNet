using Microsoft.Kiota.Abstractions;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;
using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Services;

public class ApiAuthorsService(PubNetApiClient apiClient) : IAuthorsService
{
	public async Task<AuthorDto?> GetAuthorAsync(string name, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await apiClient.Authors[name].GetAsync(cancellationToken: cancellationToken);

			if (result is null)
				throw new InvalidResponseException("Response could not be deserialized.");

			return result;
		}
		catch (NotFoundErrorDto)
		{
			return null;
		}
		catch (ApiException e)
		{
			throw InvalidResponseException.UnexpectedResponse(e);
		}
	}

	public async Task<AuthorListDto?> GetAuthorsAsync(string? query = null, int? page = null, int? perPage = null,
		CancellationToken? cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
