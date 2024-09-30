using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using PubNet.Client.Abstractions;
using PubNet.Auth;
using PubNet.Auth.Models;

namespace PubNet.Client.Web.Services;

public class LoginTokenStorageAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly ILoginTokenStorage loginTokenStorage;
	private readonly NavigationManager navigationManager;
	private readonly ILogger<LoginTokenStorageAuthenticationStateProvider> logger;

	public LoginTokenStorageAuthenticationStateProvider(
		ILoginTokenStorage loginTokenStorage,
		NavigationManager navigationManager,
		ILogger<LoginTokenStorageAuthenticationStateProvider> logger)
	{
		this.loginTokenStorage = loginTokenStorage;
		this.navigationManager = navigationManager;
		this.logger = logger;

		loginTokenStorage.TokenChanged += OnTokenChanged;
	}

	private void OnTokenChanged(object? sender, TokenChangedEventArgs e)
	{
		var principal = new ClaimsPrincipal();

		if (e.Token is { } token)
			principal.AddIdentity(GetIdentityFromToken(token));

		var state = new AuthenticationState(principal);

		NotifyAuthenticationStateChanged(Task.FromResult(state));

		navigationManager.NavigateTo(e.Token is null ? "/login" : "/");
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var principal = new ClaimsPrincipal();

		var maybeLoginToken = await loginTokenStorage.GetTokenAsync();
		if (maybeLoginToken is not { } loginToken)
			return new(principal);

		principal.AddIdentity(GetIdentityFromToken(loginToken));

		return new(principal);
	}

	private ClaimsIdentity GetIdentityFromToken(JsonWebToken loginToken)
	{
		var identity = new ClaimsIdentity("loginToken", JwtClaims.AuthorUsername, null);

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
