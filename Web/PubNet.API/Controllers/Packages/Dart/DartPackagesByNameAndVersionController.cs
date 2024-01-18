using Microsoft.AspNetCore.Mvc;
using PubNet.API.Abstractions.Packages.Dart;
using PubNet.API.DTO.Packages.Dart;

namespace PubNet.API.Controllers.Packages.Dart;

[ApiController]
[Route("packages/dart/{name}/{version}")]
public class DartPackagesByNameAndVersionController(IDartPackageVersionAnalysisProvider analysisProvider) : ControllerBase
{
	[HttpGet("analysis")]
	public async Task<DartPackageVersionAnalysisDto> GetAnalysisAsync(string name, string version, CancellationToken cancellationToken = default)
	{
		var analysis = await analysisProvider.GetAnalysisAsync(name, version, cancellationToken);

		return DartPackageVersionAnalysisDto.MapFrom(analysis);
	}
}
