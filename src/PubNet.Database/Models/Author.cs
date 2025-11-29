using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PubNet.Database.Models;

public class Author : IdentityUser<int>
{
	[Key] public new int Id { get; set; }

	[Required] public string Name { get; set; } = string.Empty;

#pragma warning disable CS8765
	[Required] public override string UserName { get; set; } = string.Empty;

	[ProtectedPersonalData, Required] public override string Email { get; set; } = string.Empty;
#pragma warning restore CS8765

	public string? Website { get; set; }

	public bool Inactive { get; set; }

	public DateTimeOffset RegisteredAtUtc { get; set; }

	public ICollection<Package> Packages { get; set; } = new List<Package>();
}
