using NuGet.Packaging.Core;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Packages.Nuget;

[Mapper]
public partial class NugetRepositoryMetadataDto
{
	public static partial NugetRepositoryMetadataDto MapFrom(RepositoryMetadata metadata);
}
