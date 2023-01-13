namespace PubNet.API.DTO;

public record ErrorResponse(ErrorResponseBody Error);
public record ErrorResponseBody(string Code, string Message);
