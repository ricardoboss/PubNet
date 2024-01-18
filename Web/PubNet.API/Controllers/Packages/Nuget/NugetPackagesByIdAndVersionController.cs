using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Nuget;

[ApiController]
[Route("packages/nuget/{id}/{version}")]
public class NugetPackagesByIdController : ControllerBase
{

}
