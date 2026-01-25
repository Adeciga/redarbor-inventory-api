using Inventory.Api.Auth;
using Inventory.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokenService;

    public AuthController(JwtTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<AuthTokenResponse> CreateToken(AuthTokenRequest request)
    {
        if (!IsValidCredentials(request))
        {
            return Unauthorized();
        }

        var token = _tokenService.CreateToken(request.Username);
        return Ok(new AuthTokenResponse(token, 7200));
    }

    private static bool IsValidCredentials(AuthTokenRequest request) =>
        request.Username == "admin" && request.Password == "admin";
}
