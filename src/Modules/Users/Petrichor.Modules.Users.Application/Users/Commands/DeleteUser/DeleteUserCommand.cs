using ErrorOr;
using MediatR;

namespace Petrichor.Modules.Users.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId, bool DeleteUploadedImages) : IRequest<ErrorOr<Deleted>>;