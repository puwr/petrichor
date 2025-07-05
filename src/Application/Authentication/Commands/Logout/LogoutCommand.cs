using ErrorOr;
using Mediator;

namespace Application.Authentication.Commands.Logout;

public record LogoutCommand(string? RefreshToken) : IRequest<ErrorOr<Success>>;