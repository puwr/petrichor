using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Images.Persistence;

public class UploadsRepository : IUploadsRepository
{
    private readonly string _dataFolder = Path
        .Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Petrichor");

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        var folderPath = Path.Combine(_dataFolder, "uploads");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(folderPath, fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);

        return $"/uploads/{fileName}";
    }

    public Task RemoveFileAsync(string filePath)
    {
        File.Delete(Path.Combine(_dataFolder, filePath.Substring(1)));

        return Task.CompletedTask;
    }
}
