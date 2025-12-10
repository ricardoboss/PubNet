using JetBrains.Annotations;

namespace PubNet.API.DTO;

[PublicAPI]
public record SuccessResponseDto(SuccessResponseBody Success);

[PublicAPI]
public record SuccessResponseBody(string Message);
