using PubNet.Web.Abstractions.Models;

namespace PubNet.Web.Abstractions;

public static class Scopes
{
	public static readonly Scope Any = Scope.From(Scope.Any);

	public static class PersonalAccessToken
	{
		public static readonly Scope Prefix = Scope.From("pat");

		public static readonly Scope Any = Prefix.WithAny();

		public static readonly Scope Create = Prefix + "create";
	}
}
