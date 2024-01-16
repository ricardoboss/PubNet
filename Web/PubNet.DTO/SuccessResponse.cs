namespace PubNet.API.DTO;

public record SuccessResponse(SuccessResponseBody Success);

public record SuccessResponseBody(string Message);
