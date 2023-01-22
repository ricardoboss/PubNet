using Microsoft.AspNetCore.Mvc;
using PubNet.API.DTO;
using PubNet.API.Services;

namespace PubNet.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthenticationController(JwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(LoginRequest dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    [HttpGet("login/google")]
    public async Task<IActionResult> LoginWithGoogle([FromQuery] string authCode, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}