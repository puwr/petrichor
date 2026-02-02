using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.TestUtilities;
using Wolverine;

namespace Petrichor.Services.Gallery.Tests.IntegrationMessageHandlers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UserRegisteredIntegrationEventHandlerTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly GalleryDbContext _dbContext;
    private readonly IMessageBus _bus;

    public UserRegisteredIntegrationEventHandlerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<GalleryDbContext>();
        _bus = _scope.ServiceProvider.GetRequiredService<IMessageBus>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task Handle_CreatesUserSnapshot()
    {
        var testUserId = Guid.NewGuid();

        await _bus.PublishAsync(new UserRegisteredIntegrationEvent(
            testUserId,
            $"UserName-{testUserId}"));

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var userSnapshot = await _dbContext.UserSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == testUserId);

            return userSnapshot is not null;
        });
    }
}