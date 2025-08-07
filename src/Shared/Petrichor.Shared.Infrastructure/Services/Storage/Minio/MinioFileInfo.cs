using Microsoft.Extensions.FileProviders;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Petrichor.Shared.Infrastructure.Common.Extensions;

namespace Petrichor.Shared.Infrastructure.Services.Storage.Minio;

public class MinioFileInfo : IFileInfo
{
    private readonly IMinioClient _minio;
    private readonly string _bucketName;
    private readonly string _objectName;
    private ObjectStat? _objectStat;

    public MinioFileInfo(IMinioClient minio, string bucketName, string objectName)
    {
        _minio = minio;
        _bucketName = bucketName;
        _objectName = objectName;
        InitializeStat();
    }

    public bool Exists => _objectStat is not null;

    public bool IsDirectory => false;

    public DateTimeOffset LastModified => _objectStat?.LastModified ?? DateTimeOffset.MinValue;

    public long Length => _objectStat?.Size ?? 0;

    public string Name => _objectName;

    public string? PhysicalPath => null;

    public Stream CreateReadStream()
    {
        var stream = new MemoryStream();

        _minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(_objectName)
            .WithCallbackStream(data => data.CopyTo(stream))
        ).GetAwaiter().GetResult();

        stream.Reset();

        return stream;
    }

    private void InitializeStat()
    {
        try
        {
            _objectStat = _minio.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(_objectName)
            ).GetAwaiter().GetResult();
        }
        catch
        {
            _objectStat = null;
        }
    }
}