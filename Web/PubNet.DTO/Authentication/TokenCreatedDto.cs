﻿using PubNet.Web.Models;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Authentication;

[Mapper]
public partial class TokenCreatedDto
{
	[MapProperty(nameof(JsonWebToken.Value), nameof(Token))]
	public static partial TokenCreatedDto MapFrom(JsonWebToken token);

	public required string Token { get; set; }
}
