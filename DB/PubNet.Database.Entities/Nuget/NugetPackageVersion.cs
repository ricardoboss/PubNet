using Microsoft.EntityFrameworkCore;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Nuget;

[EntityTypeConfiguration<NugetPackageVersionConfiguration, NugetPackageVersion>]
public class NugetPackageVersion : BasePackageVersion<NugetPackage>;
