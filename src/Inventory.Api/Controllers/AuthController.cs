using Inventory.Api.Auth;
using Inventory.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[SwaggerTag("Authentication and authorization operations")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokenService;

    public AuthController(JwtTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Generates a JWT access token using user credentials.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/token
    ///     {
    ///         "username": "admin",
    ///         "password": "admin"
    ///     }
    ///
    /// </remarks>
    /// <param name="request">User credentials</param>
    /// <returns>JWT token with expiration information</returns>
    /// <response code="200">Token generated successfully</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("token")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Generate JWT token",
        Description = "Authenticates the user and returns a JWT access token"
    )]
    [ProducesResponseType(typeof(AuthTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<AuthTokenResponse> CreateToken(
        [FromBody, SwaggerRequestBody("User credentials", Required = true)]
        AuthTokenRequest request)
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
