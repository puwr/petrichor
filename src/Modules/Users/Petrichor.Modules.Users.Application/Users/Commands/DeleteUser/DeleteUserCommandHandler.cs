using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Petrichor.Modules.Users.Application.Common.Interfaces;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Modules.Users.IntegrationMessages;
using Petrichor.Shared.Outbox;

namespace Petrichor.Modules.Users.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(
    UserManager<User> userManager,
    EventPublisher<IUsersDbContext> eventPublisher)
    : IRequestHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null)
        {
            return Result.Deleted;
        }

        user.IsDeleted = true;

        eventPublisher.Publish(new UserDeletedIntegrationEvent(
            user.Id,
            command.DeleteUploadedImages));

        await userManager.UpdateAsync(user);

        return Result.Deleted;
    }
}