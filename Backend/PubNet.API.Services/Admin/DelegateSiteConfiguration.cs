﻿using Microsoft.Extensions.Configuration;
using PubNet.API.Abstractions;
using PubNet.API.Abstractions.Admin;

namespace PubNet.API.Services;

public class DelegateSiteConfiguration(IConfiguration configuration) : ISiteConfiguration
{
	public bool RegistrationsOpen
	{
		get
		{
			var openValue = configuration.GetSection("RegistrationsOpen").Value;

			return openValue is not null && bool.Parse(openValue);
		}
	}
}
