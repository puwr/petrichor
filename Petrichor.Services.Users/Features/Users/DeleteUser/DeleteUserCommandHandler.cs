using Microsoft.AspNetCore.Identity;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.IntegrationMessages;
using Wolverine;

namespace Petrichor.Services.Users.Features.Users.DeleteUser;

public static class DeleteUserCommandHandler
{
    public static async Task Handle(
        DeleteUserCommand command,
        UserManager<User> userManager,
        IMessageBus bus)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null || user.IsDeleted)
        {
            return;
        }

        user.IsDeleted = true;

        await bus.PublishAsync(new UserDeletedIntegrationEvent(
            user.Id,
            command.DeleteUploadedImages));

        await userManager.UpdateAsync(user);
    }
}