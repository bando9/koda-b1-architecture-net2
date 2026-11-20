using System.IdentityModel.Tokens.Jwt;
using KodaStore.Application.DTOs;
using KodaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;

namespace KodaStore.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]

public sealed class AuthController(IAuthService authService, IJwtTokenService jwtTokenService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthUserResponse>> Register([FromBody] RegisterRequest request,CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request,ct);

        if(result.Status == RegisterStatus.Created)
        {
            return StatusCode(StatusCodes.Status201Created, result.User);
        }

        if(result.Status == RegisterStatus.DuplicateEmail)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Email terdaftar",
                Detail = "email sudah digunakan."
            });
        }

        var problem = new ValidationProblemDetails(new Dictionary<string, string[]>
        {
           ["registration"] = result.Error 
        })
        {
            Status = StatusCodes.Status400BadRequest,
            Title="Registration failed"
        };
        return BadRequest(problem);
    }


    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request,ct);

        if(!result.Succeeeded || result.User is null)
        {
            return Unauthorized(new ProblemDetails
            {
               Status = StatusCodes.Status401Unauthorized,
               Title = "Invalid access",
               Detail = "email atau password salah" 
            });
        }

        var user = result.User!;

        var issueToken = _jwtTokenService.IssuedAccessToken(user);

        var response = new LoginResponse("Akun berhasil diverifikasi", 
                            User: user, 
                            AccessToken: issueToken.AccessToken,
                            TokenType: "Bearer",
                            ExpiresAtUtc: issueToken.ExpiredAtUtc);

        return Ok(response);
    }


    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status401Unauthorized)]
    public ActionResult<CurrentUserResponse> Me()
    {
        var subject =  User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if(!Guid.TryParse(subject, out var userId))
        {
            return Unauthorized(new ProblemDetails
            {
                
                Status = StatusCodes.Status401Unauthorized,
                Title = "Invalid user data"
            });
        }

        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? string.Empty;

        var response = new CurrentUserResponse(userId, email); 

        return Ok(response);


    }


}