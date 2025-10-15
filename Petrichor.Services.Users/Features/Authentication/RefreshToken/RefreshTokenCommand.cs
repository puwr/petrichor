using ErrorOr;
using MediatR;

namespace Petrichor.Services.Users.Features.Authentication.RefreshToken;

public record RefreshTokenCommand(string? RefreshToken) : IRequest<ErrorOr<Success>>;