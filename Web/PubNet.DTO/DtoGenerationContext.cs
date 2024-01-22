using System.Text.Json;
using System.Text.Json.Serialization;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.DTO;

#region Authentication
[JsonSerializable(typeof(CreateAccountDto))]
[JsonSerializable(typeof(CreateLoginTokenDto))]
[JsonSerializable(typeof(CreatePersonalAccessTokenDto))]
[JsonSerializable(typeof(TokenCreatedDto))]
#endregion

#region Packages

#region Dart
[JsonSerializable(typeof(DartPackageVersionAnalysisDto))]
#endregion

#region Nuget
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
#endregion

#endregion

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
internal partial class DtoGenerationContext : JsonSerializerContext;
