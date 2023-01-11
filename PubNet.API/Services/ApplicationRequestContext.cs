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
        return AuthorToken ?? throw new BearerTokenException("Missing 'Authenticate' header. Get a token at [POST /author/token] and send it as a Bearer token");
    }

    public Author RequireAuthor()
    {
        return Author ?? throw new BearerTokenException("Missing 'Authenticate' header. Get a token at [POST /author/token] and send it as a Bearer token");
    }
}
