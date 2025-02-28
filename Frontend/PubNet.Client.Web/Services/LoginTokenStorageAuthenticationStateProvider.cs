using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using PubNet.Client.Abstractions;
using PubNet.Auth;
using PubNet.Auth.Models;

namespace PubNet.Client.Web.Services;

public class LoginTokenStorageAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly ILoginTokenStorage loginTokenStorage;
	private readonly IAuthenticationService authService;
	private readonly ILogger<LoginTokenStorageAuthenticationStateProvider> logger;

	private bool tokenValidated;

	public LoginTokenStorageAuthenticationStateProvider(
		ILoginTokenStorage loginTokenStorage,
		IAuthenticationService authService,
		ILogger<LoginTokenStorageAuthenticationStateProvider> logger)
	{
		this.loginTokenStorage = loginTokenStorage;
		this.authService = authService;
		this.logger = logger;

		loginTokenStorage.TokenChanged += OnTokenChanged;
	}

	private void OnTokenChanged(object? sender, TokenChangedEventArgs e)
	{
		tokenValidated = false;

		if (e.Token is not { } token)
		{
			var state = new AuthenticationState(new());

			NotifyAuthenticationStateChanged(Task.FromResult(state));

			return;
		}

		NotifyAuthenticationStateChanged(Task.Run(async () =>
		{
			var principal = new ClaimsPrincipal();

			var valid = await ValidateTokenAsync(token);
			if (valid)
				principal.AddIdentity(GetIdentityFromToken(token));

			return new AuthenticationState(principal);
		}));
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var principal = new ClaimsPrincipal();

		var maybeLoginToken = await loginTokenStorage.GetTokenAsync();
		if (maybeLoginToken is not { } loginToken)
			return new(principal);

		if (tokenValidated || await ValidateTokenAsync())
			principal.AddIdentity(GetIdentityFromToken(loginToken));

		return new(principal);
	}

	private async Task<bool> ValidateTokenAsync(JsonWebToken? token = null, CancellationToken cancellationToken = default)
	{
		try
		{
			// will throw if the token is invalid
			_ = await authService.GetSelfAsync(token, cancellationToken);

			tokenValidated = true;

			logger.LogTrace("Token validated");

			return true;
		}
		catch
		{
			logger.LogWarning("Token validation failed");

			await loginTokenStorage.DeleteTokenAsync(cancellationToken);

			return false;
		}
	}

	private ClaimsIdentity GetIdentityFromToken(JsonWebToken loginToken)
	{
		var identity = new ClaimsIdentity("loginToken", JwtClaims.AuthorUsername, JwtClaims.Role);

		try
		{
			identity.AddClaims(loginToken.EnumerateClaims());
		}
		catch (Exception e)
		{
			logger.LogWarning(e, "Failed to read claims from login token");

			return identity;
		}

		return identity;
	}
}
