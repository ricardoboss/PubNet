namespace PubNet.Auth;

// ReSharper disable MemberHidesStaticFromOuterClass
public static class Scopes
{
	public const char ScopeLayerSeparator = ':';

	public static class PersonalAccessTokens
	{
		private const string Prefix = "pat:";

		public const string Create = Prefix + "create";

		public const string Read = Prefix + "read";

		public const string Delete = Prefix + "delete";

		public const string Any = Prefix + "*";
	}

	public static class Packages
	{
		private const string Prefix = "packages:";

		public const string Search = Prefix + "search";

		public static class Dart
		{
			private const string Prefix = Packages.Prefix + "dart:";

			public const string New = Prefix + "new";

			public const string Discontinue = Prefix + "discontinue";

			public const string Retract = Prefix + "retract";

			public const string Search = Prefix + "search";

			public const string Any = Prefix + "*";
		}

		public static class Nuget
		{
			private const string Prefix = Packages.Prefix + "nuget:";

			public const string New = Prefix + "new";

			public const string Delete = Prefix + "delete";

			public const string Search = Prefix + "search";

			public const string Any = Prefix + "*";
		}
	}

	public static class Authors
	{
		private const string Prefix = "author:";

		public const string Search = Prefix + "search";

		public static class Delete
		{
			private const string Prefix = Authors.Prefix + "delete:";

			public const string Self = Prefix + "self";

			public const string Any = Prefix + "*";
		}
	}
}
