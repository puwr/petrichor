using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Services.Users.Features.Authentication.Register;
using Petrichor.Services.Users.Features.Users.GetUser;
using Petrichor.Services.Users.Tests.TestUtilities;

namespace Petrichor.Services.Users.Tests.Features.Users.GetUser;

[Trait("Category", "Integration")]
[Collection(ApiFactoryCollection.Name)]
public class GetUserEndpointTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task GetUser_ReturnsOk()
    {
        using var client = apiFactory.CreateClient();

        var testUserName = $"test{Random.Shared.Next(1, 10000)}";
        var registerResponse = await client.PostAsJsonAsync("/auth/register", new RegisterRequest(
            Email: $"{testUserName}@example.com",
            UserName: testUserName,
            Password: "Pa$$w0rd"));
        var registeredUserId = await registerResponse.Content.ReadFromJsonAsync<Guid>();

        var response = await client.GetAsync($"/users/{registeredUserId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<GetUserResponse>();
        user.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUser_WithNonExistentId_ReturnsNotFound()
    {
        using var client = apiFactory.CreateClient();

        var response = await client.GetAsync($"/users/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }
}