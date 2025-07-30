namespace Petrichor.Modules.Gallery.Application.Common.Interfaces.Services;

public interface IThumbnailGenerator
{
    Task<Stream> CreateThumbnailAsync(Stream imageStream);
}