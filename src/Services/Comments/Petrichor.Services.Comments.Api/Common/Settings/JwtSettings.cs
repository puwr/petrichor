namespace Petrichor.Services.Comments.Api.Common.Settings;

public sealed class JwtSettings
{
    public const string Key = "JwtSettings";

    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpirationTimeInMinutes { get; set; }
    public int RefreshTokenTokenExpirationTimeInDays { get; set; }
}