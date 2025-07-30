namespace Petrichor.Modules.Gallery.Application.Common.Interfaces.Services;

public interface IImageMetadataProvider
{
    Task<(int width, int height)> GetDimensionsAsync(Stream imageStream);
}