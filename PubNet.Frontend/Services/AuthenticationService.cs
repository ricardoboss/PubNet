using Blazored.LocalStorage;

namespace PubNet.Frontend.Services;

public class AuthenticationService
{
    private const string TokenStorageName = "authentication.token";

    private readonly ILocalStorageService _localStorage;

    public AuthenticationService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    private string? _token;

    public async ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        return _token ??= await _localStorage.GetItemAsync<string?>(TokenStorageName, cancellationToken);
    }

    public async ValueTask<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default)
    {
        return await GetTokenAsync(cancellationToken) is not null;
    }
}
