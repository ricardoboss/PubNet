using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace PubNet.Frontend.Services;

public class ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
{
	public string? Token
	{
		get => httpClient.DefaultRequestHeaders.Authorization?.Parameter;
		set
		{
			if (value is null)
				httpClient.DefaultRequestHeaders.Authorization = null;
			else
				httpClient.DefaultRequestHeaders.Authorization = new("Bearer", value);
		}
	}

	public string? BaseAddress
	{
		get => BaseUri?.ToString();
		init => BaseUri = value is not null ? new(value) : null;
	}

	private Uri? BaseUri
	{
		get => httpClient.BaseAddress;
		init => httpClient.BaseAddress = value;
	}

	public async Task<HttpResponseMessage> GetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
	{
		logger.LogTrace("[GET] {Uri}", uri);

		return await httpClient.GetAsync(uri, cancellationToken);
	}

	public async Task<T?> GetAsync<T>([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
	{
		logger.LogDebug("[GET] {Uri}", uri);

		return await httpClient.GetFromJsonAsync<T>(uri, cancellationToken);
	}

	public async Task<HttpResponseMessage> PostAsync<T>([StringSyntax(StringSyntaxAttribute.Uri)] string uri, T body, CancellationToken cancellationToken = default)
	{
		logger.LogDebug("[POST] {Uri}", uri);

		return await httpClient.PostAsJsonAsync(uri, body, cancellationToken);
	}

	public async Task<HttpResponseMessage> PatchAsync([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
	{
		logger.LogDebug("[PATCH] {Uri}", uri);

		return await httpClient.PatchAsync(uri, null, cancellationToken);
	}

	public async Task<HttpResponseMessage> PatchAsync<T>([StringSyntax(StringSyntaxAttribute.Uri)] string uri, T body, CancellationToken cancellationToken = default)
	{
		logger.LogDebug("[PATCH] {Uri}", uri);

		return await httpClient.PatchAsJsonAsync(uri, body, cancellationToken);
	}

	public async Task<HttpResponseMessage> DeleteAsync([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
	{
		logger.LogDebug("[DELETE] {Uri}", uri);

		return await httpClient.DeleteAsync(uri, cancellationToken);
	}
}
