using Petrichor.Services.Gallery.Common.Domain;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Events;

namespace Petrichor.Services.Gallery.IntegrationMessageHandlers;

public class UserRegisteredIntegrationEventHandler(GalleryDbContext dbContext)
    : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    public async Task Handle(
        UserRegisteredIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        var userSnapshot = UserSnapshot.Create(@event.UserId, @event.UserName);

        dbContext.UserSnapshots.Add(userSnapshot);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}