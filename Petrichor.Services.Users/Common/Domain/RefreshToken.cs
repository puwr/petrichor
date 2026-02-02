using Petrichor.Services.Users.Common.Services;

namespace Petrichor.Services.Users.Common.Domain;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public Guid UserId { get; private set; }

    public User User { get; private set; }

    public static RefreshToken Create(TokenResult tokenResult, Guid userId)
    {
        return new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            Token = tokenResult.Token,
            ExpiresAtUtc = tokenResult.ExpiresAtUtc,
            UserId = userId
        };
    }

    public void Renew(TokenResult tokenResult)
    {
        Token = tokenResult.Token;
        ExpiresAtUtc = tokenResult.ExpiresAtUtc;
    }
}