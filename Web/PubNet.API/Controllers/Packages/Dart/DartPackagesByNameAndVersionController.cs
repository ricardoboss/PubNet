using Microsoft.AspNetCore.Mvc;

namespace PubNet.API.Controllers.Packages.Dart;

[ApiController]
[Route("packages/dart/{name}/{version}")]
public class DartPackagesByNameAndVersionController : ControllerBase
{

}
