using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Outbox;

namespace Petrichor.Services.Users.Features.Users.DeleteUser;

public class DeleteUserCommandHandler(
    UserManager<User> userManager,
    EventPublisher<UsersDbContext> eventPublisher)
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