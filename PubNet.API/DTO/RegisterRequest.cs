namespace PubNet.API.DTO;

public record RegisterRequest(string Email, string Password, string Name, string? Website = null);
