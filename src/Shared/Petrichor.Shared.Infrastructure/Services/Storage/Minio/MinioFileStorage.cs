using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.DataModel.Args;
using Petrichor.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Shared.Infrastructure.Common.Extensions;

namespace Petrichor.Shared.Infrastructure.Services.Storage.Minio;

public class MinioFileStorage(IMinioClient minio) : IFileStorage
{
    public async Task<string> SaveFileAsync(Stream fileStream, string fileExtension, string folderName)
    {
        fileStream.Reset();

        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var contentType = new FileExtensionContentTypeProvider()
            .TryGetContentType(fileExtension, out string? mimeType)
                ? mimeType
                : "application/octet-stream";

        await minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(folderName)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType));

        return $"/{folderName}/{fileName}";
    }

    public async Task DeleteFileAsync(string filePath)
    {
        var parts = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var bucketName = parts[0];
        var objectName = parts[1];

        await minio.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName));
    }
}