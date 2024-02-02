namespace PubNet.Web;

// ReSharper disable MemberHidesStaticFromOuterClass
public static class Scopes
{
	public static class PersonalAccessTokens
	{
		private const string Prefix = "pat:";

		public const string Create = Prefix + "create";
	}

	public static class Dart
	{
		private const string Prefix = "dart:";

		public const string New = Prefix + "new";

		public const string Discontinue = Prefix + "discontinue";

		public const string Retract = Prefix + "retract";
	}
}
