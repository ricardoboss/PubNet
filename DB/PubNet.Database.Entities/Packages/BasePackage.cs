﻿namespace PubNet.Database.Entities.Packages;

public abstract class BasePackage<TVersion> where TVersion : class
{
	public Guid Id { get; init; }

	public Guid AuthorId { get; init; }

	public virtual Author Author { get; init; } = null!;

	public string Name { get; init; } = null!;

	public Guid? LatestVersionId { get; init; }

	public virtual TVersion? LatestVersion { get; set; }

	public virtual ICollection<TVersion> Versions { get; init; } = [];

}
