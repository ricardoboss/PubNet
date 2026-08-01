namespace PubNet.Database.Models;

/// <summary>
/// The privilege level of an <see cref="Author"/>. The values are spaced out so new roles can be inserted
/// between existing ones without a data migration.
/// </summary>
public enum Role
{
	Default = 0,

	Admin = 1337,
}
