using System.Net;
using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Gallery.Tests.IntegrationMessageHandlers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class UserDeletedIntegrationEventHandlerTests : IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly IBus _bus;

    public UserDeletedIntegrationEventHandlerTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _bus = _scope.ServiceProvider.GetRequiredService<IBus>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task Handle_DeletesImages()
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
            userId: testUserId,
            deleteUploadedImages: true);
        await _bus.Publish(userDeletedEvent);

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
            userId: testUserId,
            deleteUploadedImages: false);
        await _bus.Publish(userDeletedEvent);

        await Poller.PollAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var getImageResponse = await client.GetAsync($"images/{uploadImageId}");

            return getImageResponse.StatusCode == HttpStatusCode.OK;
        });
    }
}