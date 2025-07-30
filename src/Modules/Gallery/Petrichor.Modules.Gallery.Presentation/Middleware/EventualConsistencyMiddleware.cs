using MediatR;
using Microsoft.AspNetCore.Http;
using Petrichor.Shared.Domain.Common;
using Petrichor.Modules.Gallery.Infrastructure.Persistence;

namespace Petrichor.Modules.Gallery.Presentation.Middleware;

internal class EventualConsistencyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IPublisher publisher,
        GalleryDbContext dbContext)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();

        context.Response.OnCompleted(async () =>
        {
            try
            {
                if (context.Items.TryGetValue("DomainEventsQueue", out var value) &&
                    value is Queue<IDomainEvent> domainEventsQueue)
                {
                    while (domainEventsQueue.TryDequeue(out var domainEvent))
                    {
                        await publisher.Publish(domainEvent);
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        });

        await next(context);
    }
}