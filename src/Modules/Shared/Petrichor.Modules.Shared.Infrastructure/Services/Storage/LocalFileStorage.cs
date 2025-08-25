using Petrichor.Modules.Shared.Application.Common.Interfaces.Services.Storage;
using Petrichor.Modules.Shared.Infrastructure.Common.Extensions;

namespace Petrichor.Modules.Shared.Infrastructure.Services.Storage;

public class LocalFileStorage(string? dataFolder = null) : IFileStorage
{
    private readonly string _dataFolder = dataFolder
        ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Petrichor");

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileExtension,
        string folderName)
    {
        fileStream.Reset();

        var folderPath = Path.Combine(_dataFolder, folderName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{Guid.CreateVersion7()}{fileExtension}";
        var filePath = Path.Combine(folderPath, fileName);

        await using var file = File.Create(filePath);
        await fileStream.CopyToAsync(file);

        return $"/{folderName}/{fileName}";
    }

    public Task DeleteFileAsync(string filePath)
    {
        var sanitizedFilePath = Path.Combine(filePath.Split("/"));

        File.Delete(Path.Combine(_dataFolder, sanitizedFilePath));

        return Task.CompletedTask;
    }
}