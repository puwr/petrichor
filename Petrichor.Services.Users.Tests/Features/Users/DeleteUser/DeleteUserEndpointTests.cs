using System.Net;
using System.Net.Http.Json;
using Petrichor.Services.Users.Features.Authentication.Register;
using Petrichor.Services.Users.Features.Users.GetUser;
using Petrichor.Services.Users.Tests.TestUtilities;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Users.Tests.Features.Users.DeleteUser;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class DeleteUserEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task DeleteUser_ReturnsNoContent()
    {
        using var client = apiFactory.CreateClient();
        var testUserName = $"test{Random.Shared.Next(1, 10000)}";
        var registerResponse = await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: $"{testUserName}@example.com",
            UserName: testUserName,
            Password: "Pa$$w0rd"));
        var registeredUserId = await registerResponse.Content.ReadFromJsonAsync<Guid>();

        using var adminClient = apiFactory.CreateClient();
        adminClient.SetFakeClaims(role: "Admin");

        var response = await adminClient.DeleteAsync($"/users/{registeredUserId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deletedUser = await adminClient.GetFromJsonAsync<GetUserResponse>($"/users/{registeredUserId}");
        deletedUser.Should().NotBeNull();
        deletedUser.UserName.Should().Be("Deleted User");
    }

    [Fact]
    public async Task DeleteUser_WithNonExistentId_ReturnsNoContent()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims(role: "Admin");

        var response = await client.DeleteAsync($"/users/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUser_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.DeleteAsync($"/users/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteUser_WhenNotAdmin_ReturnsForbidden()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var response = await client.DeleteAsync($"/users/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}