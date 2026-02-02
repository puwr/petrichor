using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Users.Features.Authentication.Register;
using Petrichor.Services.Users.Features.Users.GetUsers;
using Petrichor.Services.Users.Tests.TestUtilities;
using Petrichor.Shared.Pagination;
using Petrichor.TestUtilities.Authentication;

namespace Petrichor.Services.Users.Tests.Features.Users.GetUsers;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class GetUsersEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task GetUsers_ReturnsOk()
    {
        using var client = apiFactory.CreateClient();
        var testUserName = $"test{Random.Shared.Next(1, 10000)}";
        await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: $"{testUserName}@example.com",
            UserName: testUserName,
            Password: "Pa$$w0rd"));

        using var adminClient = apiFactory.CreateClient();
        adminClient.SetFakeClaims(role: "Admin");

        var response = await adminClient.GetAsync($"/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<PagedResponse<GetUsersResponse>>();
        users.Should().NotBeNull();
        users.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetUsers_WhenAnonymous_ReturnsUnauthorized()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.GetAsync($"/users");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUsers_WhenNotAdmin_ReturnsForbidden()
    {
        using var client = apiFactory.CreateClient();
        client.SetFakeClaims();

        var response = await client.GetAsync($"/users");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}