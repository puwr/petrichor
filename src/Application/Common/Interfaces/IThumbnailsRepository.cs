namespace Application.Common.Interfaces;

public interface IThumbnailsRepository
{
    Task<string> GenerateAndSaveThumbnail(string imagePath);
    Task RemoveThumbnail(string thumbnailPath);
}