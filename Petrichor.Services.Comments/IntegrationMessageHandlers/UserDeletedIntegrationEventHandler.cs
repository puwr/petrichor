using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Comments.Common.Persistence;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Events;

namespace Petrichor.Services.Comments.IntegrationMessageHandlers;

public class UserDeletedIntegrationEventHandler(CommentsDbContext dbContext)
    : IIntegrationEventHandler<UserDeletedIntegrationEvent>
{
    public async Task Handle(
        UserDeletedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Comments
            .Where(c => c.AuthorId == @event.UserId)
            .ExecuteDeleteAsync(cancellationToken);

        await dbContext.UserSnapshots
            .Where(c => c.UserId == @event.UserId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}