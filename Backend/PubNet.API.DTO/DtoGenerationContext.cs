using System.Text.Json;
using System.Text.Json.Serialization;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.API.DTO.Packages.Nuget;
using PubNet.API.DTO.Packages.Nuget.Spec;

namespace PubNet.API.DTO;

#region Authentication
[JsonSerializable(typeof(CreateAccountDto))]
[JsonSerializable(typeof(CreateLoginTokenDto))]
[JsonSerializable(typeof(CreatePersonalAccessTokenDto))]
[JsonSerializable(typeof(TokenCollectionDto))]
[JsonSerializable(typeof(TokenCreatedDto))]
[JsonSerializable(typeof(TokenDto))]
#endregion

#region Authors
[JsonSerializable(typeof(AuthorDto))]
[JsonSerializable(typeof(AuthorListDto))]
#endregion

#region Packages

[JsonSerializable(typeof(PackageListCollectionDto))]
[JsonSerializable(typeof(PackageVersionDto))]

#region Dart
[JsonSerializable(typeof(DartMessageDto))]
[JsonSerializable(typeof(DartArchiveUploadInformationDto))]
[JsonSerializable(typeof(DartPackageDto))]
[JsonSerializable(typeof(DartPackageVersionDto))]
[JsonSerializable(typeof(DartSuccessDto))]
[JsonSerializable(typeof(DartPackageVersionAnalysisDto))]
[JsonSerializable(typeof(DartPackageListDto))]
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
[JsonSerializable(typeof(NugetPackageDto))]
[JsonSerializable(typeof(NugetPackageListDto))]
[JsonSerializable(typeof(NugetPackageVersionDto))]
#endregion

#endregion

#region Errors

[JsonSerializable(typeof(AuthErrorDto))]
[JsonSerializable(typeof(GenericErrorDto))]
[JsonSerializable(typeof(InternalServerErrorDto))]
[JsonSerializable(typeof(InvalidRoleErrorDto))]
[JsonSerializable(typeof(MissingScopeErrorDto))]
[JsonSerializable(typeof(ValidationErrorsDto))]

#endregion

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
public partial class DtoGenerationContext : JsonSerializerContext;
