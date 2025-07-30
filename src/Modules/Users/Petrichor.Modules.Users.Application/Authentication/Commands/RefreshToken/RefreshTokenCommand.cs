using ErrorOr;
using MediatR;

namespace Petrichor.Modules.Users.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string? RefreshToken) : IRequest<ErrorOr<Success>>;