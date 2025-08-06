using Microsoft.AspNetCore.Identity;
using Petrichor.Shared.Domain.Common;

namespace Petrichor.Modules.Users.Domain.Users;

public class User : IdentityUser<Guid>, IHasDomainEvents
{
    protected readonly List<DomainEvent> _domainEvents = [];

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    public DateTime RegisteredAtUtc { get; init; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public User(string email, string userName)
    {
        Email = email;
        UserName = userName;
    }

    public User() { }

    public List<DomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();

        _domainEvents.Clear();

        return copy;
    }
}