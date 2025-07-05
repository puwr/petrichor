using ErrorOr;
using Mediator;

namespace Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string? RefreshToken) : IRequest<ErrorOr<Success>>;