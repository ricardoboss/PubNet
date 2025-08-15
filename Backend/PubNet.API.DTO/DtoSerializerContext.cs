using System.Text.Json;
using System.Text.Json.Serialization;
using PubNet.API.DTO.Authentication;
using PubNet.API.DTO.Authors;
using PubNet.API.DTO.Errors;
using PubNet.API.DTO.Packages;
using PubNet.API.DTO.Packages.Dart;
using PubNet.API.DTO.Packages.Dart.Spec;

namespace PubNet.API.DTO;

#region Authentication
[JsonSerializable(typeof(CreateAccountDto))]
[JsonSerializable(typeof(CreateLoginTokenDto))]
[JsonSerializable(typeof(CreatePersonalAccessTokenDto))]
[JsonSerializable(typeof(TokenCollectionDto))]
[JsonSerializable(typeof(TokenCreatedDto))]
[JsonSerializable(typeof(TokenDto))]
[JsonSerializable(typeof(AccountCreatedDto))]
#endregion

#region Authors
[JsonSerializable(typeof(AuthorDto))]
[JsonSerializable(typeof(AuthorListDto))]
[JsonSerializable(typeof(DeleteAuthorDto))]
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

#endregion

#region Errors

[JsonSerializable(typeof(AuthErrorDto))]
[JsonSerializable(typeof(GenericErrorDto))]
[JsonSerializable(typeof(InternalServerErrorDto))]
[JsonSerializable(typeof(InvalidRoleErrorDto))]
[JsonSerializable(typeof(MissingScopeErrorDto))]
[JsonSerializable(typeof(ValidationErrorsDto))]
[JsonSerializable(typeof(NotFoundErrorDto))]

#endregion

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
public partial class DtoSerializerContext : JsonSerializerContext;
