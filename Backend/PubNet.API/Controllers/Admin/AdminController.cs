using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubNet.API.Attributes;
using PubNet.Auth.Models;

namespace PubNet.API.Controllers.Admin;

[ApiController]
[Route("[controller]")]
[Authorize, RequireRole(Role.Admin)]
public class AdminController() : PubNetControllerBase
{
}
