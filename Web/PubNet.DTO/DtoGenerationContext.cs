using System.Text.Json.Serialization;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.DTO;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DartPackageVersionAnalysisDto))]
[JsonSerializable(typeof(NugetAlternatePackageDto))]
[JsonSerializable(typeof(NugetAlternatePackageDto))]
[JsonSerializable(typeof(NugetAlternatePackageDto))]
[JsonSerializable(typeof(NugetAutocompleteResultDto))]
[JsonSerializable(typeof(NugetCatalogEntryDto))]
[JsonSerializable(typeof(NugetPackageDependencyDto))]
[JsonSerializable(typeof(NugetPackageDependencyGroupDto))]
[JsonSerializable(typeof(NugetPackageDeprecationDto))]
[JsonSerializable(typeof(NugetPackageIndexDto))]
[JsonSerializable(typeof(NugetPackageRegistrationIndexDto))]
[JsonSerializable(typeof(NugetPackageRegistrationLeafDto))]
[JsonSerializable(typeof(NugetPackageRegistrationPageDto))]
[JsonSerializable(typeof(NugetPackageVulnerabilityDto))]
[JsonSerializable(typeof(NugetSearchResultDto))]
[JsonSerializable(typeof(NugetSearchResultHitDto))]
[JsonSerializable(typeof(NugetSearchResultHitPackageTypeDto))]
[JsonSerializable(typeof(NugetSearchResultHitVersionInfoDto))]
[JsonSerializable(typeof(NugetServiceIndexDto))]
[JsonSerializable(typeof(NugetServiceIndexResourceDto))]
[JsonSerializable(typeof(NugetVulnerabilityIndexEntryDto))]
internal partial class DtoGenerationContext : JsonSerializerContext;
