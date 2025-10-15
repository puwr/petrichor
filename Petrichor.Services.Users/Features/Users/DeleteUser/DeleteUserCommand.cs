using ErrorOr;
using MediatR;

namespace Petrichor.Services.Users.Features.Users.DeleteUser;

public record DeleteUserCommand(Guid UserId, bool DeleteUploadedImages)
    : IRequest<ErrorOr<Deleted>>;