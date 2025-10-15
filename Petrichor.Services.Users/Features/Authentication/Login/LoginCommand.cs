using ErrorOr;
using MediatR;

namespace Petrichor.Services.Users.Features.Authentication.Login;

public record LoginCommand(string Email, string Password) : IRequest<ErrorOr<Success>>;