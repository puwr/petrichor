using Application.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Images.Persistence;

public class ThumbnailsRepository : IThumbnailsRepository
{
    private readonly string _dataFolder = Path
        .Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Petrichor");

    public async Task<string> GenerateAndSaveThumbnail(string filePath)
    {
        var folderPath = Path.Combine(_dataFolder, "thumbs");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        using Image image = await Image.LoadAsync(Path.Combine(_dataFolder, filePath.Substring(1)));

        int width = 450;
        image.Mutate(x => x.Resize(width, height: 0, KnownResamplers.Lanczos3));

        var thumbnailName = $"{Guid.NewGuid()}.jpg";

        await image.SaveAsJpegAsync(Path.Combine(folderPath, thumbnailName));

        return $"/thumbs/{thumbnailName}";
    }

    public Task RemoveThumbnail(string thumbnailPath)
    {
        throw new NotImplementedException();
    }
}
