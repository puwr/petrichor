using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Gallery.Common.Domain;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;

namespace Petrichor.Services.Gallery.IntegrationMessageHandlers;

public static class UserRegisteredIntegrationEventHandler
{
    public static async Task Handle(
        UserRegisteredIntegrationEvent @event,
        GalleryDbContext dbContext,
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