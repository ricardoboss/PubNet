using PubNet.Database.Entities.Dart;
using Riok.Mapperly.Abstractions;

namespace PubNet.API.DTO.Packages.Dart.Spec;

[Mapper]
public partial class DartPubSpecDto
{
	public static partial DartPubSpecDto MapFrom(PubSpec pubspec);
}
