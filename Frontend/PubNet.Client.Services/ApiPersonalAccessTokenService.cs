﻿using System.Net;
using System.Text;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;
using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Services;

public class ApiPersonalAccessTokenService(PubNetApiClient apiClient) : IPersonalAccessTokenService
{
	public async Task<IEnumerable<TokenDto>> GetAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var result =
				await apiClient.Authentication.PersonalAccessToken.GetAsync(cancellationToken: cancellationToken);

			if (result is not { Tokens: { } tokens })
				throw new InvalidResponseException("No response could be deserialized");

			// filter out current token
			if (result.CurrentTokenId is { } currentTokenId)
				return tokens.Where(t => t.Id != currentTokenId);

			return tokens;
		}
		catch (ApiException e) when (e.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
		{
			throw new UnauthorizedAccessException("Authentication is required for this request", e);
		}
		catch (ApiException e)
		{
			throw InvalidResponseException.UnexpectedResponse(e);
		}
	}

	public async Task<(TokenDto token, string jwt)> CreateAsync(CreatePersonalAccessTokenDto dto, CancellationToken cancellationToken = default)
	{
		try
		{
			var result =
				await apiClient.Authentication.PersonalAccessToken.PostAsync(dto, cancellationToken: cancellationToken);

			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			if (result is not { Token: { } token, Value: { } jwt })
				throw new InvalidResponseException("The response did not contain a token");

			return (token, jwt);
		}
		catch (ApiException e) when (e.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
		{
			throw new UnauthorizedAccessException("Authentication is required for this request", e);
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
			throw InvalidResponseException.UnexpectedResponse(e);
		}
	}

	public async Task<IReadOnlyCollection<string>> GetAllowedScopesAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await apiClient.Authentication.PersonalAccessToken.AllowedScopes.GetAsync(cancellationToken: cancellationToken);

			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			return result;
		}
		catch (ApiException e)
		{
			throw InvalidResponseException.UnexpectedResponse(e);
		}
	}

	public async Task DeleteTokenAsync(Guid tokenId, CancellationToken cancellationToken = default)
	{
		try
		{
			await apiClient.Authentication.PersonalAccessToken.DeleteAsync(r =>
			{
				r.QueryParameters.TokenId = tokenId.ToString("D");
			}, cancellationToken);
		}
		catch (ApiException e) when (e.ResponseStatusCode == (int)HttpStatusCode.NotFound)
		{
			throw new TokenNotFoundException(e.Message);
		}
		catch (ApiException e)
		{
			throw InvalidResponseException.UnexpectedResponse(e);
		}
	}
}
