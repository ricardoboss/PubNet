using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace PubNet.Frontend.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
        get => _httpClient.BaseAddress?.ToString();
        set => _httpClient.BaseAddress = value is not null ? new(value) : null;
    }

    public async Task<HttpResponseMessage> GetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string uri, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync(uri, cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync<T>([StringSyntax(StringSyntaxAttribute.Uri)] string uri, T body, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsJsonAsync(uri, body, cancellationToken);
    }
}
