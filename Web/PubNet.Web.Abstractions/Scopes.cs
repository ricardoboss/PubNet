﻿namespace PubNet.Web.Abstractions;

public static class Scopes
{
	public static class PersonalAccessTokens
	{
		private const string Prefix = "pat:";

		public const string Create = Prefix + "create";
	}
}
