using Petrichor.Modules.Users.IntegrationMessages;
using Petrichor.Services.Comments.Api.Common.Domain;
using Petrichor.Services.Comments.Api.Common.Persistence;
using Petrichor.Shared.Events;

namespace Petrichor.Services.Comments.Api.IntegrationMessageHandlers;

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