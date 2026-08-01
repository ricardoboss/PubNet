using PubNet.API.DTO;

namespace PubNet.API.Services;

/// <summary>
/// A registration was rejected because of what the caller sent. Carries the response to return, so every
/// endpoint creating an account reports the same code for the same problem.
/// </summary>
public sealed class AuthorRegistrationException(ErrorResponse response) : Exception(response.Error?.Message)
{
	public ErrorResponse Response { get; } = response;

	public string Code => Response.Error?.Code ?? "invalid-request";
}
