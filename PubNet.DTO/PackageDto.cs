using PubNet.Database.Models;

namespace PubNet.API.DTO;

public record PackageDto(string Name, IEnumerable<PackageVersionDto>? Versions, bool IsDiscontinued, string? ReplacedBy, PackageVersionDto? Latest, AuthorDto? Author)
{
    public static PackageDto FromPackage(Package package)
    {
        return new(
            package.Name,
            package.Versions.Any() ? package.Versions.Select(PackageVersionDto.FromPackageVersion) : null,
            package.IsDiscontinued,
            package.ReplacedBy,
            package.Latest is null ? null : PackageVersionDto.FromPackageVersion(package.Latest),
            AuthorDto.FromAuthor(package.Author, true)
        );
    }
}
