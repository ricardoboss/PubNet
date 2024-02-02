using PubNet.Web.Models;

namespace PubNet.API.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RequireScopeAttribute(string require) : Attribute
{
	public Scope Scope { get; } = Scope.From(require);
}
