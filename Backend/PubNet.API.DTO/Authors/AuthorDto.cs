﻿using System.ComponentModel.DataAnnotations;
using PubNet.Database.Entities;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Authors;

[Mapper]
public partial class AuthorDto
{
	public static partial AuthorDto MapFrom(Author author);

	[Required]
	public required Guid Id { get; init; }

	[Required]
	public required string UserName { get; init; }

	[Required]
	public required DateTimeOffset RegisteredAtUtc { get; init; }
}