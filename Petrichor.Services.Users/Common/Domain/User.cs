using Microsoft.AspNetCore.Identity;

namespace Petrichor.Services.Users.Common.Domain;

public class User : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    public DateTime RegisteredAtUtc { get; init; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public static User Create(string email, string userName)
    {
        return new User
        {
            Email = email,
            UserName = userName
        };
    }
}