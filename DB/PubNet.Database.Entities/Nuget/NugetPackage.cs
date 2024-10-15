using Microsoft.EntityFrameworkCore;
using PubNet.Database.Entities.Packages;

namespace PubNet.Database.Entities.Nuget;

[EntityTypeConfiguration<NugetPackageConfiguration, NugetPackage>]
public class NugetPackage : BasePackage<NugetPackageVersion>;
