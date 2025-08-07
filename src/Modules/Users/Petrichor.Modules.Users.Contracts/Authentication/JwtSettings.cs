namespace Petrichor.Modules.Users.Contracts.Authentication;

public sealed class JwtSettings
{
    public const string Key = "Users:JwtSettings";

    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpirationTimeInMinutes { get; set; }
    public int RefreshTokenTokenExpirationTimeInDays { get; set; }
}