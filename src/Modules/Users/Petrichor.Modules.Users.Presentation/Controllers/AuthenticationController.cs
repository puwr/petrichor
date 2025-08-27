using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Modules.Shared.Presentation;
using Petrichor.Modules.Users.Application.Authentication.Commands.Login;
using Petrichor.Modules.Users.Application.Authentication.Commands.Logout;
using Petrichor.Modules.Users.Application.Authentication.Commands.RefreshToken;
using Petrichor.Modules.Users.Application.Authentication.Commands.Register;
using Petrichor.Modules.Users.Contracts.Authentication;

namespace Petrichor.Modules.Users.Presentation.Controllers;

[AllowAnonymous]
[Route("auth")]
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