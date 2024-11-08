﻿using System.Net;
using Microsoft.Kiota.Abstractions;
using PubNet.Client.Abstractions;
using PubNet.Client.ApiClient.Generated;
using PubNet.Client.ApiClient.Generated.Models;

namespace PubNet.Client.Services;

public class ApiDartPackagesService(PubNetApiClient apiClient) : IDartPackagesService
{
	public async Task<DartPackageDto?> GetPackageAsync(string name, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await apiClient.Packages.Dart[name].GetAsync(cancellationToken: cancellationToken);

			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			return result;
		}
		catch (ApiException e) when (e.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
		{
			throw new UnauthorizedAccessException("Authentication is required for this request", e);
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

	public async Task<DartPackageVersionDto?> GetPackageVersionAsync(string name, string? version,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await apiClient.Packages.Dart[name].Versions[version ?? "latest"]
				.GetAsync(cancellationToken: cancellationToken);

			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			return result;
		}
		catch (ApiException e) when (e.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
		{
			throw new UnauthorizedAccessException("Authentication is required for this request", e);
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

	public async Task<DartPackageListDto> GetPackagesAsync(string? query = null, int? page = null, int? perPage = null,
		CancellationToken? cancellationToken = default)
	{
		try
		{
			var result = await apiClient.Packages.Dart.Search.GetAsync(r =>
			{
				r.QueryParameters.Q = query;
				r.QueryParameters.Skip = page * perPage;
				r.QueryParameters.Take = perPage;
			});

			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			return result;
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

	public async Task<DartPackageListDto> ByAuthorAsync(string author, string? query = null, int? page = null,
		int? perPage = null,
		CancellationToken? cancellationToken = default)
	{
		try
		{
			var result = await apiClient.Authors[author].Packages.Dart.GetAsync(r =>
			{
				r.QueryParameters.Q = query;
				r.QueryParameters.Skip = page * perPage;
				r.QueryParameters.Take = perPage;
			});

			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

			return result;
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

	public async Task<DartPackageVersionAnalysisDto?> GetPackageVersionAnalysisAsync(string name, string version,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await apiClient.Packages.Dart[name].Versions[version].AnalysisJson
				.GetAsync(cancellationToken: cancellationToken);

			if (result is null)
				throw new InvalidResponseException("No response could be deserialized");

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
}
