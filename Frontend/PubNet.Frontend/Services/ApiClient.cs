using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace PubNet.Frontend.Services;

public class ApiClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<ApiClient> _logger;

	public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public string? Token
	{
		get => _httpClient.DefaultRequestHeaders.Authorization?.Parameter;
		set
		{
			if (value is null)
				_httpClient.DefaultRequestHeaders.Authorization = null;
			else
				_httpClient.DefaultRequestHeaders.Authorization = new("Bearer", value);
		}
	}

	public string? BaseAddress
	{
		get => BaseUri?.ToString();
		set => BaseUri = value is not null ? new(value) : null;
	}

	public Uri? BaseUri
	{
		get => _httpClient.BaseAddress;
		set => _httpClient.BaseAddress = value;
	}

	public async Task<HttpResponseMessage> GetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
	{
		_logger.LogTrace("[GET] {Uri}", uri);

		return await _httpClient.GetAsync(uri, cancellationToken);
	}

	public async Task<T?> GetAsync<T>([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("[GET] {Uri}", uri);

		return await _httpClient.GetFromJsonAsync<T>(uri, cancellationToken);
	}

	public async Task<HttpResponseMessage> PostAsync<T>([StringSyntax(StringSyntaxAttribute.Uri)] string uri, T body, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("[POST] {Uri}", uri);

		return await _httpClient.PostAsJsonAsync(uri, body, cancellationToken);
	}

	public async Task<HttpResponseMessage> PatchAsync([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("[PATCH] {Uri}", uri);

		return await _httpClient.PatchAsync(uri, null, cancellationToken);
	}

	public async Task<HttpResponseMessage> PatchAsync<T>([StringSyntax(StringSyntaxAttribute.Uri)] string uri, T body, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("[PATCH] {Uri}", uri);

		return await _httpClient.PatchAsJsonAsync(uri, body, cancellationToken);
	}

	public async Task<HttpResponseMessage> DeleteAsync([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
	{
		_logger.LogDebug("[DELETE] {Uri}", uri);

		return await _httpClient.DeleteAsync(uri, cancellationToken);
	}
}
