using PubNet.Database.Models;

namespace PubNet.API.DTO;

public record PackageVersionDto(string Version, string PackageName, bool Retracted, string ArchiveUrl, string ArchiveSha256, DateTimeOffset PublishedAt, PubSpec? PubSpec, PackageVersionAnalysisDto? Analysis)
{
    public static PackageVersionDto FromPackageVersion(PackageVersion version)
    {
        return new(
            version.Version,
            version.PackageName,
            version.Retracted,
            version.ArchiveUrl,
            version.ArchiveSha256,
            version.PublishedAtUtc,
            version.PubSpec,
            PackageVersionAnalysisDto.FromPackageVersionAnalysis(version.Analysis)
        );
    }
}
