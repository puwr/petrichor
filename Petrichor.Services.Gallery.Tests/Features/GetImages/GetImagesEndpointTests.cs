using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Gallery.Common.Persistence;
using Petrichor.Services.Gallery.Features.AddImageTags;
using Petrichor.Services.Gallery.Features.GetImages;
using Petrichor.Services.Gallery.Tests.TestUtilities;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Shared.Pagination;
using Petrichor.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Gallery.Tests.Features.GetImages;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class GetImagesEndpointTests: IDisposable
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly GalleryDbContext _dbContext;
    private readonly IBus _bus;

    public GetImagesEndpointTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<GalleryDbContext>();
        _bus = _scope.ServiceProvider.GetRequiredService<IBus>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task GetImages_ReturnsOk()
    {
        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims();

        await using var imageStream = TestImages.GetImageStream("test-image.jpg");
        var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };
        var uploadImageResponse = await client.PostAsync("/images", formData);
        uploadImageResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await client.GetAsync("/images");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var images = await response.Content.ReadFromJsonAsync<PagedResponse<GetImagesResponse>>();
        images.Should().NotBeNull();
        images.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetImages_WithTagsQuery_ReturnsOkWithMatchingImages()
    {
        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims();

        await using var imageStream = TestImages.GetImageStream("test-image.jpg");
        var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };
        var uploadImageResponse = await client.PostAsync("/images", formData);
        uploadImageResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        var tag = $"tag-{Guid.NewGuid()}";
        var addImageTagResponse = await client
            .PostAsJsonAsync($"/images/{uploadedImageId}/tags", new AddImageTagsRequest([tag]));
        addImageTagResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var response = await client.GetAsync($"/images?tags={tag}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var images = await response.Content.ReadFromJsonAsync<PagedResponse<GetImagesResponse>>();
        images.Should().NotBeNull();
        images.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetImages_WithUploaderQuery_ReturnsOkWithMatchingImages()
    {
        var testUserId = Guid.NewGuid();
        var testUserName = "test123";

        await _bus.Publish(new UserRegisteredIntegrationEvent(testUserId, testUserName));

        await Poller.WaitAsync(TimeSpan.FromSeconds(10), async () =>
        {
            var userSnapshot = await _dbContext.UserSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == testUserId);

            return userSnapshot is not null;
        });

        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims(new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, testUserId },
            { JwtRegisteredClaimNames.UniqueName, testUserName }
        });

        await using var imageStream = TestImages.GetImageStream("test-image.jpg");
        var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };
        var uploadImageResponse = await client.PostAsync("/images", formData);
        var uploadedImageId = await uploadImageResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await client.GetAsync($"/images?uploader={testUserName}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var images = await response.Content.ReadFromJsonAsync<PagedResponse<GetImagesResponse>>();
        images.Should().NotBeNull();
        images.Count.Should().Be(1);
        images.Items.Count.Should().Be(1);
        images.Items[0].Id.Should().Be(uploadedImageId);
    }
}