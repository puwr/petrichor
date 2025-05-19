namespace Infrastructure.Authentication;

public class JwtSettings
{
    public const string JwtSettingsKey = "JwtSettings";

    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessTokenExpirationTimeInMinutes { get; set; }
    public int RefreshTokenTokenExpirationTimeInDays { get; set; }
}