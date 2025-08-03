using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Petrichor.Shared.Infrastructure.Serialization;

namespace Petrichor.Shared.Infrastructure.Outbox;

public class OutboxBackgroudService<TDbContext>(
    IServiceProvider services,
    ILogger<OutboxBackgroudService<TDbContext>> logger) : BackgroundService
    where TDbContext : IOutboxDbContext
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var moduleName = typeof(TDbContext).Namespace?.Split('.')[2] ?? "Unknown";

            await using var scope = services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            var outboxMessages = await dbContext.OutboxMessages
                .Where(om => om.ProcessedAtUtc == null)
                .OrderBy(om => om.OccurredAtUtc)
                .Take(50)
                .ToListAsync(cancellationToken);

            if (outboxMessages.Count > 0)
                logger.LogInformation(
                    "[{Module}] - Processing {MessageCount} outbox messages",
                    moduleName,
                    outboxMessages.Count);

            foreach (var message in outboxMessages)
            {
                try
                {
                    var domainEvent = DomainEventsSerializer.Deserialize(message.Content);

                    await publisher.Publish(domainEvent, cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "[{Module}] - Failed to process outbox message {MessageId}",
                        moduleName,
                        message.Id);

                    message.Error = exception?.ToString();
                }

                message.ProcessedAtUtc = DateTime.UtcNow;

                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[{Module}] - Successfully processed {SuccessCount} out of {TotalCount} outbox messages",
                    moduleName,
                    outboxMessages.Count(om => om.Error == null),
                    outboxMessages.Count);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }
}