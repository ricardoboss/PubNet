using PubNet.Auth.Models;

namespace PubNet.API.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RequireAnyScopeAttribute(params string[] scopes) : Attribute
{
	public Scope[] Scopes { get; } = scopes.Select(Scope.From).ToArray();
}
