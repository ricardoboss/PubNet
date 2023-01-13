using PubNet.API.DTO;
using PubNet.API.Models;

namespace PubNet.API.Services;

public class ApplicationRequestContext
{
    public List<string> AcceptedApiVersions { get; } = new();

    public List<string> AcceptedResponseFormats { get; } = new();

    public Author? Author => AuthorToken?.Owner;

    public AuthorToken? AuthorToken { get; set; }

    public AuthorToken RequireAuthorToken()
    {
        return AuthorToken ?? throw MissingHeader;
    }

    public Author RequireAuthor()
    {
        return Author ?? throw MissingHeader;
    }

    private BearerTokenException MissingHeader => new("Missing authentication. Acquire a Bearer token at [POST /authors/{username}/tokens] and send it in the 'Authenticate' header.");
}
