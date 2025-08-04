using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Petrichor.Modules.Users.Domain.Users;

namespace Petrichor.Modules.Users.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(UserManager<User> userManager)
    : IRequestHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null)
        {
            return Error.NotFound(description: "User not found.");
        }

        user.IsDeleted = true;
        await userManager.UpdateAsync(user);

        return Result.Deleted;
    }
}
