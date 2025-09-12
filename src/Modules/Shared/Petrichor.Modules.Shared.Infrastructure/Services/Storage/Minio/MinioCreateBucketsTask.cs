using Microsoft.Extensions.Hosting;
using Minio;
using Minio.DataModel.Args;

namespace Petrichor.Modules.Shared.Infrastructure.Services.Storage.Minio;

public class MinioCreateBucketsTask(IMinioClient minio) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateBucket("uploads", cancellationToken);
        await CreateBucket("thumbs", cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task CreateBucket(string bucketName, CancellationToken cancellationToken)
    {
        var bucketExists = await minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucketName), cancellationToken);

        if (!bucketExists)
        {
            await minio.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucketName), cancellationToken);
        }
    }
}