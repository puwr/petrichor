using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Petrichor.Shared.Domain.Common;
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
            var bus = scope.ServiceProvider.GetRequiredService<IBus>();

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
                    if (message.Type == OutboxEventType.Domain)
                    {
                        var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(
                            message.Content,
                            SerializerSettings.Events)!;

                        await publisher.Publish(domainEvent, cancellationToken);
                    }
                    else if (message.Type == OutboxEventType.Integration)
                    {
                        var integrationEventType = Type.GetType(message.AssemblyQualifiedName);

                        var integrationEvent = JsonConvert.DeserializeObject(
                            message.Content,
                            integrationEventType,
                            SerializerSettings.Events)!;

                        await bus.Publish(integrationEvent, cancellationToken);
                    }
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

            await Task.Delay(2000, cancellationToken);
        }
    }
}