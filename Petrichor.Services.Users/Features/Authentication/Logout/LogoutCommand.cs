using ErrorOr;
using MediatR;

namespace Petrichor.Services.Users.Features.Authentication.Logout;

public record LogoutCommand(string? RefreshToken) : IRequest<ErrorOr<Success>>;