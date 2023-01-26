﻿using System.Text.Json.Serialization;

namespace PubNet.Models;

public class Package
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsDiscontinued { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReplacedBy { get; set; }

    public PackageVersion? Latest { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<PackageVersion> Versions { get; set; } = new List<PackageVersion>();

    [JsonIgnore]
    public int? AuthorId { get; set; }

    public Author? Author { get; set; }
}