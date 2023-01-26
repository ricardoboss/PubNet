using System.Security.Authentication;
using System.Security.Claims;
using PubNet.API.Contexts;
using PubNet.Models;

namespace PubNet.API.Services;

public class ApplicationRequestContext
{
    public List<string> AcceptedApiVersions { get; } = new();

    public List<string> AcceptedResponseFormats { get; } = new();

    public Author? Author { get; set; }

    public async Task<Author> RequireAuthorAsync(ClaimsPrincipal user, PubNetContext db, CancellationToken cancellationToken = default)
    {
        if (Author is not null)
            return Author;

        var idStr = user.FindFirstValue("id");
        if (idStr is null || !int.TryParse(idStr, out var id))
            throw MissingAuthentication;

        var author = await db.Authors.FindAsync(new object?[] { id }, cancellationToken);
        if (author is null)
            throw MissingAuthentication;

        return Author = author;
    }

    private static InvalidCredentialException MissingAuthentication => new("Missing authentication. Acquire a Bearer token at [POST /authentication/login] and send it in the 'Authenticate' header.");
}
