using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.TestUtilities;
using Petrichor.TestUtilities.Authentication;
using Wolverine;

namespace Petrichor.Services.Gallery.Tests.IntegrationMessageHandlers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UserDeletedIntegrationEventHandlerTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly GalleryDbContext _dbContext;
    private readonly IMessageBus _bus;

    public UserDeletedIntegrationEventHandlerTests(ApiFactory apiFactory)
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
    public async Task Handle_DeleteUsersSnapshot()
    {
        var testUserId = Guid.NewGuid();

        await _bus.PublishAsync(new UserRegisteredIntegrationEvent(testUserId, $"UserName-{testUserId}"));
        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var userSnapshot = await _dbContext.UserSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == testUserId);

            return userSnapshot is not null;
        });

        await _bus.PublishAsync(new UserDeletedIntegrationEvent(testUserId, false));

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var userSnapshot = await _dbContext.UserSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == testUserId);

            return userSnapshot is null;
        });
    }

    [Fact]
    public async Task Handle_WhenDeleteUploadedImagesIsTrue_DeletesImages()
    {
        var testUserId = Guid.NewGuid();

        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims(userId: testUserId);

        await using var imageStream = TestImages.GetImageStream("test-image.jpg");
        var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };
        var uploadImageResponse = await client.PostAsync("/images", formData);
        uploadImageResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var uploadImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();
        uploadImageId.Should().NotBeEmpty();

        var userDeletedEvent = new UserDeletedIntegrationEvent(
            UserId: testUserId,
            DeleteUploadedImages: true);
        await _bus.PublishAsync(userDeletedEvent);

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var getImageResponse = await client.GetAsync($"images/{uploadImageId}");

            return getImageResponse.StatusCode == HttpStatusCode.NotFound;
        });
    }

    [Fact]
    public async Task Handle_WhenDeleteUploadedImagesIsFalse_ImagesRemain()
    {
        var testUserId = Guid.NewGuid();

        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims(userId: testUserId);

        await using var imageStream = TestImages.GetImageStream("test-image.jpg");
        var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };
        var uploadImageResponse = await client.PostAsync("/images", formData);
        uploadImageResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var uploadImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();
        uploadImageId.Should().NotBeEmpty();

        var userDeletedEvent = new UserDeletedIntegrationEvent(
            UserId: testUserId,
            DeleteUploadedImages: false);
        await _bus.PublishAsync(userDeletedEvent);

        await Poller.PollAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var getImageResponse = await client.GetAsync($"images/{uploadImageId}");

            return getImageResponse.StatusCode == HttpStatusCode.OK;
        });
    }
}