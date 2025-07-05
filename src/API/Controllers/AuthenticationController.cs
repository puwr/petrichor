using Application.Authentication.Commands.Login;
using Application.Authentication.Commands.Logout;
using Application.Authentication.Commands.RefreshToken;
using Application.Authentication.Commands.Register;
using Contracts.Authentication;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[AllowAnonymous]
[Route("api/v{version:apiVersion}/auth")]
public class AuthenticationController(
    ISender mediator,
    IHttpContextAccessor httpContextAccessor) : ApiController
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.Email, request.UserName, request.Password);

        var registerResult = await mediator.Send(command);

        return registerResult.Match(
            success => Ok(),
            Problem
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginCommand(request.Email, request.Password);

        var loginResult = await mediator.Send(query);

        return loginResult.Match(
            success => Ok(),
            Problem
        );
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = _httpContext.Request.Cookies["REFRESH_TOKEN"];

        var command = new RefreshTokenCommand(refreshToken);

        var refreshTokenResult = await mediator.Send(command);

        return refreshTokenResult.Match(
            success => Ok(),
            Problem
        );
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = _httpContext.Request.Cookies["REFRESH_TOKEN"];

        var command = new LogoutCommand(refreshToken);

        var logoutResult = await mediator.Send(command);

        return logoutResult.Match(
            success => NoContent(),
            Problem
        );
    }

    [HttpGet("status")]
    public IActionResult GetAuthenticationStatus()
    {
        return Ok(new AuthenticationStatusResponse(
            User.Identity?.IsAuthenticated ?? false));
    }
}