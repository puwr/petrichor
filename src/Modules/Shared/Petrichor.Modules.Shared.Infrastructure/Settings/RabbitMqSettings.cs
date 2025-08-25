namespace Petrichor.Modules.Shared.Infrastructure.Settings;

public class RabbitMqSettings
{
    public const string Key = "Queue";

    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}