using System.Security.Authentication;
using System.Security.Claims;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Services;

public class ApplicationRequestContext
{
	private Author? _author;

	private static InvalidCredentialException MissingAuthentication => new(
		"Missing authentication. Acquire a Bearer token at [POST /authentication/login] and send it in the 'Authenticate' header.");

	public async Task<Author> RequireAuthorAsync(ClaimsPrincipal user, PubNetContext db,
		CancellationToken cancellationToken = default)
	{
		if (_author is not null)
			return _author;

		var idStr = user.FindFirstValue("id");
		if (idStr is null || !int.TryParse(idStr, out var id))
			throw MissingAuthentication;

		var author = await db.Authors.FindAsync([id], cancellationToken);
		if (author is null)
			throw MissingAuthentication;

		return _author = author;
	}
}
