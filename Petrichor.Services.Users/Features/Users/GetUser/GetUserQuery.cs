using ErrorOr;
using MediatR;

namespace Petrichor.Services.Users.Features.Users.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<ErrorOr<GetUserResponse>>;