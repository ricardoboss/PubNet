namespace PubNet.Web;

// ReSharper disable MemberHidesStaticFromOuterClass
public static class Scopes
{
	public static class PersonalAccessTokens
	{
		private const string Prefix = "pat:";

		public const string Create = Prefix + "create";

		public const string Read = Prefix + "read";
	}

	public static class Dart
	{
		private const string Prefix = "dart:";

		public const string New = Prefix + "new";

		public const string Discontinue = Prefix + "discontinue";

		public const string Retract = Prefix + "retract";
	}

	public static class Nuget
	{
		private const string Prefix = "nuget:";

		public const string Delete = Prefix + "delete";
	}

	public static class Authors
	{
		private const string Prefix = "author:";

		public static class Delete
		{
			private const string Prefix = Authors.Prefix + "delete:";

			public const string Self = Prefix + "self";

			public const string Any = Prefix + "*";
		}
	}
}
