using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Petrichor.Modules.Users.Application.Common.Interfaces;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Modules.Users.IntegrationEvents;
using Petrichor.Shared.Infrastructure.Outbox;

namespace Petrichor.Modules.Users.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(UserManager<User> userManager, IUsersDbContext dbContext)
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

        dbContext.OutboxMessages.Add(OutboxMessage.From(new UserDeletedIntegrationEvent(
            user.Id,
            command.DeleteUploadedImages)));

        await userManager.UpdateAsync(user);

        return Result.Deleted;
    }
}