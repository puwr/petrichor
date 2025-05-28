namespace Application.Common.Interfaces.Services.Images;

public interface IImageMetadataProvider
{
    Task<(int width, int height)> GetDimensionsAsync(Stream imageStream);
}