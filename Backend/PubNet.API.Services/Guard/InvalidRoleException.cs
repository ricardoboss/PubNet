using PubNet.Auth.Models;

namespace PubNet.API.Services.Guard;

public class InvalidRoleException(Role claimedRole, Role requiredRole, string? message = null)
	: UnauthorizedAccessException(message ?? $"Role {claimedRole} is not allowed to perform this action")
{
	public Role ClaimedRole { get; } = claimedRole;

	public Role RequiredRole { get; } = requiredRole;
}
