using Application.Authentication.Commands.Login;
using Application.Authentication.Commands.RefreshToken;
using Application.Authentication.Commands.Register;
using Contracts.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[AllowAnonymous]
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
}