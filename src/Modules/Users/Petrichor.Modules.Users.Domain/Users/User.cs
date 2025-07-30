using Microsoft.AspNetCore.Identity;

namespace Petrichor.Modules.Users.Domain.Users;

public class User : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }

    public User(string email, string userName)
    {
        Email = email;
        UserName = userName;
    }
}