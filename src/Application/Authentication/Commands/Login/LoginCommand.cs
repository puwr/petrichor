using ErrorOr;
using MediatR;

namespace Application.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<ErrorOr<Success>>;