﻿using PubNet.Web.Models;

namespace PubNet.Web.ServiceInterfaces;

public interface IGuardThrowsBuilder
{
	public void Cannot(Scope scope, string? message = null);
}