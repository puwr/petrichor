using ErrorOr;
using MediatR;

namespace Petrichor.Modules.Users.Application.Authentication.Commands.Logout;

public record LogoutCommand(string? RefreshToken) : IRequest<ErrorOr<Success>>;