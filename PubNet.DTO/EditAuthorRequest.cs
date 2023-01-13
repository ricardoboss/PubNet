namespace PubNet.API.DTO;

public record EditAuthorRequest(string? Name, string? Website, bool RemoveWebsite = false);
