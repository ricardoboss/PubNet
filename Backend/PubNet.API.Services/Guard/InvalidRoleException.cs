using PubNet.Auth.Models;

namespace PubNet.API.Services.Guard;

public class InvalidRoleException(Role givenRole, Role requiredRole, string? message = null)
	: UnauthorizedAccessException(message ?? $"Role {givenRole} is not allowed to perform this action")
{
	public Role GivenRole { get; } = givenRole;

	public Role RequiredRole { get; } = requiredRole;
}
