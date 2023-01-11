using PubNet.API.Models;

namespace PubNet.API.Services;

public class ApplicationRequestContext
{
    public List<string> AcceptedApiVersions { get; } = new();
    public List<string> AcceptedResponseFormats { get; } = new();

    public Author? Author { get; set; }
    public AuthorToken? AuthorToken { get; set; }
}
