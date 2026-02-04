using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Features.Authentication.Register;
using Petrichor.Services.Users.IntegrationMessages;
using Petrichor.Services.Users.Tests.TestUtilities;
using Petrichor.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Users.Tests.Features.Users.DeleteUser;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class DeleteUserEndpointTests
{
    private readonly ApiFactory _apiFactory;
    private readonly IServiceScope _scope;
    private readonly UsersDbContext _dbContext;

    public DeleteUserEndpointTests(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _scope = _apiFactory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent()
    {
        using var client = _apiFactory.CreateClient();
        var testUserName = $"test{Random.Shared.Next(1, 10000)}";
        var registerResponse = await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: $"{testUserName}@example.com",
            UserName: testUserName,
            Password: "Pa$$w0rd"));
        var registeredUserId = await registerResponse.Content.ReadFromJsonAsync<Guid>();

        using var adminClient = _apiFactory.CreateClient();
        adminClient.SetFakeClaims(role: "Admin");

        var (response, session) = await _apiFactory.Services
            .TrackHttpCall(async () => await adminClient.DeleteAsync($"/users/{registeredUserId}"));
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var userDeletedEvent = session.Sent.SingleMessage<UserDeletedIntegrationEvent>();
        userDeletedEvent.UserId.Should().Be(registeredUserId);
        userDeletedEvent.DeleteUploadedImages.Should().Be(false);

        var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == registeredUserId);
        user.Should().NotBeNull();
        user.IsDeleted.Should().Be(true);
    }

    [Fact]
    public async Task DeleteUser_WithNonExistentId_ReturnsNoContent()
    {
        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims(role: "Admin");

        var response = await client.DeleteAsync($"/users/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUser_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = _apiFactory.CreateClient();

        var response = await client.DeleteAsync($"/users/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteUser_WhenNotAdmin_ReturnsForbidden()
    {
        using var client = _apiFactory.CreateClient();
        client.SetFakeClaims();

        var response = await client.DeleteAsync($"/users/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}