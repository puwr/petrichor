using Minio;
using Minio.DataModel.Args;

namespace Petrichor.Services.Gallery.Common.Storage;

public class MinioCreateBucketsTask(IMinioClient minio) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateBucket(StorageFolders.Uploads, cancellationToken);
        await CreateBucket(StorageFolders.Thumbnails, cancellationToken);
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