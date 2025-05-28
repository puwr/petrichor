namespace Application.Common.Interfaces.Services.Images;

public interface IThumbnailGenerator
{
    Task<Stream> CreateThumbnailAsync(Stream imageStream);
}