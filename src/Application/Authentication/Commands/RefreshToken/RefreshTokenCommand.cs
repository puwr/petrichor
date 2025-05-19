using ErrorOr;
using MediatR;

namespace Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string? RefreshToken) : IRequest<ErrorOr<Success>>;