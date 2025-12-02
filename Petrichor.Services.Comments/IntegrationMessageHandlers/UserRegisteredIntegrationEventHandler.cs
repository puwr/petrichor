using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Domain;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;

namespace Petrichor.Services.Comments.IntegrationMessageHandlers;

public static class UserRegisteredIntegrationEventHandler
{
    public static async Task Handle(
        UserRegisteredIntegrationEvent @event,
        CommentsDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var snapshotExists = await dbContext.UserSnapshots
            .AnyAsync(us => us.UserId == @event.UserId, cancellationToken);

        if (snapshotExists) return;

        var userSnapshot = UserSnapshot.Create(@event.UserId, @event.UserName);

        dbContext.UserSnapshots.Add(userSnapshot);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}