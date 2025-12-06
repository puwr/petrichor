namespace Petrichor.Services.Gallery.Common.Services;

public interface IThumbnailGenerator
{
    Task<Stream> CreateThumbnailAsync(Stream imageStream, CancellationToken cancellationToken = default);
}