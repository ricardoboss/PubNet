using System.Text.Json.Serialization;
using PubNet.API.DTO.Packages.Dart;

namespace PubNet.API.DTO;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DartPackageVersionAnalysisDto))]
internal partial class DtoGenerationContext : JsonSerializerContext;
