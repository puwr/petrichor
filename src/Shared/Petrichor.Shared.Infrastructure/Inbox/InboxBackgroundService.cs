using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Petrichor.Shared.Application.Common.Events;
using Petrichor.Shared.Infrastructure.Serialization;

namespace Petrichor.Shared.Infrastructure.Inbox;

public class InboxBackgroundService<TDbContext>(
    IServiceProvider services,
    ILogger<InboxBackgroundService<TDbContext>> logger) : BackgroundService
    where TDbContext : IInboxDbContext
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var moduleName = typeof(TDbContext).Namespace?.Split('.')[2] ?? "Unknown";

            await using var scope = services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            var inboxMessages = await dbContext.InboxMessages
                .Where(om => om.ProcessedAtUtc == null)
                .OrderBy(om => om.OccurredAtUtc)
                .Take(50)
                .ToListAsync(cancellationToken);

            if (inboxMessages.Count > 0)
                logger.LogInformation(
                    "[{Module}] - Processing {MessageCount} inbox messages",
                    moduleName,
                    inboxMessages.Count);

            foreach (var message in inboxMessages)
            {
                try
                {
                    var integrationEvent = JsonConvert.DeserializeObject<IntegrationEvent>(
                            message.Content,
                            SerializerSettings.Events)!;

                    await publisher.Publish(integrationEvent, cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "[{Module}] - Failed to process inbox message {MessageId}",
                        moduleName,
                        message.Id);

                    message.Error = exception?.ToString();
                }

                message.ProcessedAtUtc = DateTime.UtcNow;

                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[{Module}] - Successfully processed {SuccessCount} out of {TotalCount} inbox messages",
                    moduleName,
                    inboxMessages.Count(om => om.Error == null),
                    inboxMessages.Count);
            }

            await Task.Delay(4000, cancellationToken);
        }
    }
}