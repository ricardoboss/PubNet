using PubNet.API.DTO.Packages.Dart.Spec;
using PubNet.Database.Entities.Auth;

namespace PubNet.API.Abstractions.Packages.Dart;

public interface IDartPackageUploadService
{
	Task<DartNewVersionDto> CreateNewAsync(Token token, CancellationToken cancellationToken = default);
}
