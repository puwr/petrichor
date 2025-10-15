using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Minio;

namespace Petrichor.Shared.Services.Storage.Minio;

public class MinioFileProvider(IMinioClient minio, string bucketName) : IFileProvider
{
    public IFileInfo GetFileInfo(string subpath)
    {
        return new MinioFileInfo(minio, bucketName, subpath.TrimStart('/'));
    }

    public IDirectoryContents GetDirectoryContents(string subpath) => new NotFoundDirectoryContents();

    public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
}