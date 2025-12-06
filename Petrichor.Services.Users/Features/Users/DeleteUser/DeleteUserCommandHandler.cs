using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.IntegrationMessages;
using Wolverine;
using ZiggyCreatures.Caching.Fusion;

namespace Petrichor.Services.Users.Features.Users.DeleteUser;

public static class DeleteUserCommandHandler
{
    public static async Task<Deleted> Handle(
        DeleteUserCommand command,
        UserManager<User> userManager,
        IMessageBus bus,
        IFusionCache cache,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null)
        {
            return Result.Deleted;
        }

        user.IsDeleted = true;

        await bus.PublishAsync(new UserDeletedIntegrationEvent(
            user.Id,
            command.DeleteUploadedImages));

        await userManager.UpdateAsync(user);

        await cache.RemoveAsync($"user:{user.Id}", token: cancellationToken);

        return Result.Deleted;
    }
}