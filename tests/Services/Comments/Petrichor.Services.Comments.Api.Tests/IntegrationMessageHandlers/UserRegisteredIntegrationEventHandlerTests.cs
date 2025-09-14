using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Modules.Users.IntegrationMessages;
using Petrichor.Services.Comments.Api.Common.Persistence;
using TestUtilities;

namespace Petrichor.Services.Comments.Api.Tests.IntegrationMessageHandlers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UserRegisteredIntegrationEventHandlerTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly CommentsDbContext _dbContext;
    private readonly IBus _bus;

    public UserRegisteredIntegrationEventHandlerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<CommentsDbContext>();
        _bus = _scope.ServiceProvider.GetRequiredService<IBus>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task Handle_CreatesUserSnapshot()
    {
        var testUserId = Guid.NewGuid();
        var userRegisteredEvent = new UserRegisteredIntegrationEvent(testUserId, $"UserName-{testUserId}");
        await _bus.Publish(userRegisteredEvent);

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var userSnapshot = await _dbContext.UserSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == testUserId);

            return userSnapshot is not null;
        });
    }
}