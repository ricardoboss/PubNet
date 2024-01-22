using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Abstractions.Authentication;
using PubNet.Database.Context;
using PubNet.Database.Entities;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.Authentication;

public class HttpAuthProvider(IHttpContextAccessor contextAccessor, PubNetContext dbContext) : IAuthProvider
{
	private string? TryGetTokenValue()
	{
		var httpContext = contextAccessor.HttpContext;
		var principal = httpContext?.User.FindFirst(c => c.Type == "t"); // TODO: move this somewhere central
		return principal?.Value;
	}

	public async Task<Token?> TryGetCurrentTokenAsync(CancellationToken cancellationToken = default)
	{
		var tokenValue = TryGetTokenValue();
		if (tokenValue == null)
			return null;

		var token = await dbContext.Tokens
			.Include(t => t.Identity)
			.Include(t => t.Identity.Author)
			.SingleOrDefaultAsync(t => t.Value == tokenValue, cancellationToken);

		return token;
	}

	public async Task<Identity?> TryGetCurrentIdentityAsync(CancellationToken cancellationToken = default)
	{
		var token = await TryGetCurrentTokenAsync(cancellationToken);

		return token?.Identity;
	}

	public async Task<Author?> TryGetCurrentAuthorAsync(CancellationToken cancellationToken = default)
	{
		var token = await TryGetCurrentTokenAsync(cancellationToken);

		return token?.Identity.Author;
	}
}
