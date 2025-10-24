using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Petrichor.TestUtilities.Authentication;

public static class TestAuthenticationExtensions
{
    public static void SetFakeClaims(this HttpClient client, Guid? userId = null, string? role = null)
    {
        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, userId?.ToString() ?? Guid.NewGuid().ToString()},
            { ClaimTypes.Role, role ?? "" }
        };

        client.SetFakeClaims(claims);
    }

    public static void SetFakeClaims(this HttpClient client, Dictionary<string, object> claims)
    {
        var token = JsonSerializer.Serialize(claims);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}