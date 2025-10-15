namespace Petrichor.Shared.Services.Storage;

public interface IFileStorage
{
    Task<string> SaveFileAsync(
        Stream fileStream,
        string fileExtension,
        string folderName);

    Task DeleteFileAsync(string filePath);
}