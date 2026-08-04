# PubNet.SDK

Client library for the PubNet API.

It exposes a small set of service interfaces — packages, authors, analyses, authentication and onboarding —
implemented on top of a [Kiota](https://learn.microsoft.com/openapi/kiota/)-generated client that is regenerated
from the API's OpenAPI document at build time. Consumers depend on the interfaces and the SDK's exceptions, not
on HTTP or on the generated types.

The SDK has no UI or platform dependencies: the Blazor frontend in this repository is one consumer, a console
application is another.

## Getting started

Storing the login token is the one thing the SDK cannot decide for you, so you provide it:

```csharp
using PubNet.SDK.Abstractions;

public sealed class FileLoginTokenStorage : ILoginTokenStorage
{
    private static readonly string Path =
        System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".pubnet-token");

    public Task<string?> GetTokenAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(File.Exists(Path) ? File.ReadAllText(Path) : null);

    public async Task StoreTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await File.WriteAllTextAsync(Path, token, cancellationToken);

        TokenChanged?.Invoke(this, new(token));
    }

    public Task DeleteTokenAsync(CancellationToken cancellationToken = default)
    {
        File.Delete(Path);

        TokenChanged?.Invoke(this, new(null));

        return Task.CompletedTask;
    }

    public event EventHandler<TokenChangedEventArgs>? TokenChanged;
}
```

Then register the services against the instance you want to talk to:

```csharp
using Microsoft.Extensions.DependencyInjection;
using PubNet.SDK.Extensions;

var services = new ServiceCollection();

services.AddLogging();
services
    .AddPubNetApiServices<FileLoginTokenStorage>(new Uri("https://pubnet.example.com/api/"))
    .AddConcurrentRequestPrevention() // serialises requests so a burst cannot stampede the API
    .AddCaching();                    // remembers packages, authors and analyses for the lifetime of the scope

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
```

Requests are sent with a named `HttpClient` (`PubNetServiceCollectionExtensions.HttpClientName`), so the
default client of the consuming application is left alone. Use the overload taking
`Action<IServiceProvider, HttpClient>` if you need to configure it further.

## Using the services

```csharp
var auth = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
var packages = scope.ServiceProvider.GetRequiredService<IPackagesService>();

var token = await auth.LoginAsync("someone@example.com", "hunter2");
await scope.ServiceProvider.GetRequiredService<ILoginTokenStorage>().StoreTokenAsync(token.Token!);

var package = await packages.GetPackageAsync("my_package", includeAuthor: true);
```

## Errors

Every service translates API responses into exceptions derived from `PubNetSdkException`; the generated Kiota
error types never escape the SDK. Lookups that simply found nothing return `null` rather than throwing.

| Exception | Raised when |
| --- | --- |
| `AuthenticationRequiredException` | No token, or the API rejected it |
| `UnauthorizedException` | Authenticated, but the account may not do this |
| `EmailNotFoundException` | No account exists for the e-mail address |
| `InvalidLoginCredentialsException` | The e-mail address or password is wrong |
| `InvalidPasswordException` | An action needing password confirmation got the wrong one |
| `InvalidPasswordResetTokenException` | The password reset link is invalid, expired or already used |
| `PackageNotFoundException`, `PackageVersionNotFoundException` | The package or version does not exist |
| `LastAdminException` | The account is the instance's last administrator |
| `RegisterException` and subclasses | Registration was rejected |
| `OnboardingNotPendingException` | The instance has already been set up |
| `UnexpectedResponseException` | The API answered in a way the SDK does not model |

Each interface documents the exceptions its methods raise.
