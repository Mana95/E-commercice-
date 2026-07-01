using DevFlow.Api.DTOs;
using DevFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }
        catch (DuplicateEmailException ex)
        {
            return Conflict(new ProblemDetails { Title = "Registration failed", Detail = ex.Message, Status = StatusCodes.Status409Conflict });
        }
        catch (WeakPasswordException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Registration failed", Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new ProblemDetails { Title = "Login failed", Detail = ex.Message, Status = StatusCodes.Status401Unauthorized });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        await _authService.LogoutAsync(request.RefreshToken);
        return Ok();
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(result);
        }
        catch (InvalidRefreshTokenException ex)
        {
            return Unauthorized(new ProblemDetails { Title = "Refresh failed", Detail = ex.Message, Status = StatusCodes.Status401Unauthorized });
        }
    }
}
