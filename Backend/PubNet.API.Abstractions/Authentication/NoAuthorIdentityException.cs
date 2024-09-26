using PubNet.Database.Entities;

namespace PubNet.API.Abstractions.Authentication;

public class NoAuthorIdentityException(Author author) : Exception($"Author {author.UserName} has no identity to authenticate with.");
