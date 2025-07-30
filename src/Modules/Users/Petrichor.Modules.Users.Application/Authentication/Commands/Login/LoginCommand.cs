using ErrorOr;
using MediatR;

namespace Petrichor.Modules.Users.Application.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<ErrorOr<Success>>;