using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;

namespace Petrichor.Services.Comments.IntegrationMessageHandlers;

public static class UserDeletedIntegrationEventHandler
{
    public static async Task Handle(
        UserDeletedIntegrationEvent @event,
        CommentsDbContext dbContext,
        CancellationToken cancellationToken)
    {
        await dbContext.Comments
            .Where(c => c.AuthorId == @event.UserId)
            .ExecuteDeleteAsync(cancellationToken);

        await dbContext.UserSnapshots
            .Where(c => c.UserId == @event.UserId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}