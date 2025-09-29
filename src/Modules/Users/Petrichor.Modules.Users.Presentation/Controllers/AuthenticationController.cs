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
    [EndpointSummary("Register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.Email, request.UserName, request.Password);

        var registerResult = await mediator.Send(command);

        return registerResult.Match(
            success => NoContent(),
            Problem
        );
    }

    [HttpPost("login")]
    [EndpointSummary("Login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginCommand(request.Email, request.Password);

        var loginResult = await mediator.Send(query);

        return loginResult.Match(
            success => NoContent(),
            Problem
        );
    }

    [HttpPost("refresh-token")]
    [EndpointSummary("Refresh token")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = _httpContext.Request.Cookies["REFRESH_TOKEN"];

        var command = new RefreshTokenCommand(refreshToken);

        var refreshTokenResult = await mediator.Send(command);

        return refreshTokenResult.Match(
            success => NoContent(),
            Problem
        );
    }

    [HttpPost("logout")]
    [EndpointSummary("Logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
    [EndpointSummary("Get auth status")]
    [ProducesResponseType<AuthenticationStatusResponse>(StatusCodes.Status200OK)]
    public IActionResult GetAuthenticationStatus()
    {
        return Ok(new AuthenticationStatusResponse(
            User.Identity?.IsAuthenticated ?? false));
    }
}