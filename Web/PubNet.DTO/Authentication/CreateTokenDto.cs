﻿namespace PubNet.API.DTO.Authentication;

public class CreateTokenDto
{
	public required string Username { get; init; }

	public required string Password { get; init; }
}
