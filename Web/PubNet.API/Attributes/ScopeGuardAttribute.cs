﻿using PubNet.Web.Abstractions.Models;

namespace PubNet.API.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ScopeGuardAttribute(string require) : Attribute
{
	public Scope Require { get; } = Scope.From(require);
}
