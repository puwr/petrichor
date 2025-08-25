namespace Petrichor.Services.Comments.Api.Common.Settings;

public class RabbitMqSettings
{
    public const string Key = "Queue";

    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}