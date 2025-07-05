using ErrorOr;
using Mediator;

namespace Application.Authentication.Commands.Register;

public record RegisterCommand(
    string Email,
    string UserName,
    string Password) : IRequest<ErrorOr<Success>>;