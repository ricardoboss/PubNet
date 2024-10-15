using System.Text;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;
using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Services;

public class ApiRegisterService(PubNetApiClient apiClient) : IRegisterService
{
	public async Task<bool> AreRegistrationsOpenAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var result =
				await apiClient.Authentication.RegistrationsOpen.GetAsync(cancellationToken: cancellationToken);
			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			return result.Value;
		}
		catch (ApiException e)
		{
			throw new InvalidResponseException("API returned an unexpected status code", e);
		}
	}

	public async Task RegisterAsync(CreateAccountDto request, CancellationToken cancellationToken = default)
	{
		try
		{
			var result =
				await apiClient.Authentication.Account.PostAsync(request, cancellationToken: cancellationToken);
			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");
		}
		catch (GenericErrorDto e) when (e.ResponseStatusCode == 409)
		{
			throw new RegisterException(e.Error?.Message ?? "Username or email address already exists");
		}
		catch (ValidationErrorsDto e)
		{
			var sb = new StringBuilder();
			sb.AppendLine(e.Title!);

			foreach (var (field, errorsObj) in e.Errors!.AdditionalData)
			{
				var errors = (errorsObj as UntypedArray)?.GetValue()
					.Select(node => (node as UntypedString)?.GetValue())
					.OfType<string>();

				sb.AppendLine($"{field}:");
				foreach (var error in errors ?? ["Unknown error"])
					sb.AppendLine($"{error}");
			}

			throw new RegisterException(sb.ToString());
		}
		catch (ApiException e)
		{
			throw new InvalidResponseException("API returned an unexpected status code", e);
		}
	}
}
