using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.DataModel.Args;
using Petrichor.Shared.Extensions;

namespace Petrichor.Shared.Services.Storage.Minio;

public class MinioFileStorage(IMinioClient minio) : IFileStorage
{
    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileExtension,
        string folderName,
        CancellationToken cancellationToken = default)
    {
        fileStream.Reset();

        var fileName = $"{Guid.CreateVersion7()}{fileExtension}";
        var contentType = new FileExtensionContentTypeProvider()
            .TryGetContentType(fileExtension, out string? mimeType)
                ? mimeType
                : "application/octet-stream";

        await minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(folderName)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType), cancellationToken);

        return $"/{folderName}/{fileName}";
    }

    public async Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var parts = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var bucketName = parts[0];
        var objectName = parts[1];

        await minio.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName), cancellationToken);
    }
}