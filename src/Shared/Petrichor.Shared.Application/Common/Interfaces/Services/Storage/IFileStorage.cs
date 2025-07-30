namespace Petrichor.Shared.Application.Common.Interfaces.Services.Storage;

public interface IFileStorage
{
    Task<string> SaveFileAsync(
        Stream fileStream,
        string fileExtension,
        string folderName);

    Task DeleteFileAsync(string filePath);
}
