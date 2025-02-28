using PubNet.Auth.Models;

namespace PubNet.API.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequireRoleAttribute(Role requiredRole) : Attribute
{
	public Role Role { get; } = requiredRole;
}
