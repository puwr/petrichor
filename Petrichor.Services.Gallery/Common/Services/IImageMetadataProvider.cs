namespace Petrichor.Services.Gallery.Common.Services;

public interface IImageMetadataProvider
{
    Task<(int width, int height)> GetDimensionsAsync(Stream imageStream);
}