using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NuGet.Versioning;

namespace PubNet.Database.Entities.Converters;

public class NuGetVersionValueConverter() : ValueConverter<NuGetVersion, string>(
	v => v.ToNormalizedString(),
	v => NuGetVersion.Parse(v)
);
