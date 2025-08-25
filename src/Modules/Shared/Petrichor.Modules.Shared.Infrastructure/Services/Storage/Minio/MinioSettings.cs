namespace Petrichor.Modules.Shared.Infrastructure.Services.Storage.Minio;

public sealed class MinioSettings
{
    public const string Key = "Minio";

    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public bool UseSsl { get; set; }
}