﻿using System.Diagnostics.CodeAnalysis;
using PubNet.Auth.Models;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Services.Extensions;

public static class RoleExtensions
{
	public static string? ToClaimValue(this Role role)
	{
		return role switch
		{
			Role.Unspecified => null,
			Role.Default => "default",
			Role.Admin => "admin",
			_ => throw new NotSupportedException($"Role {role} is not supported"),
		};
	}

	[return: NotNullIfNotNull(nameof(claimValue))]
	public static Role? ToRole(this string? claimValue)
	{
		return claimValue switch
		{
			null => null,
			"default" => Role.Default,
			"admin" => Role.Admin,
			_ => throw new NotSupportedException($"Role {claimValue} is not supported"),
		};
	}
}
