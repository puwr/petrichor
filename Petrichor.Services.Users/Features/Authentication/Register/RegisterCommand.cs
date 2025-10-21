using ErrorOr;
using MediatR;

namespace Petrichor.Services.Users.Features.Authentication.Register;

public record RegisterCommand(
    string Email,
    string UserName,
    string Password) : IRequest<ErrorOr<Guid>>;