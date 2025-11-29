namespace PubNet.API.DTO;

public class BearerTokenException(string message) : UnauthorizedAccessException(message);
