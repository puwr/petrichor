using Minio;
using Petrichor.Shared.Services.Storage.Minio;

namespace Petrichor.Services.Gallery.Common.Storage;

public static class MinioExtensions
{
    public static IApplicationBuilder UseMinioStaticFiles(this IApplicationBuilder builder)
    {
        var minio = builder.ApplicationServices.GetRequiredService<IMinioClient>();

        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new MinioFileProvider(minio, StorageFolders.Uploads),
            RequestPath = $"/{StorageFolders.Uploads}"
        });

        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new MinioFileProvider(minio, StorageFolders.Thumbnails),
            RequestPath = $"/{StorageFolders.Thumbnails}"
        });

        return builder;
    }
}