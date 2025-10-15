using Petrichor.Services.Comments.Common.Domain;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Events;

namespace Petrichor.Services.Comments.IntegrationMessageHandlers;

public class UserRegisteredIntegrationEventHandler(CommentsDbContext dbContext)
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