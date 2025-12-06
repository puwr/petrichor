namespace Petrichor.Shared.Services.Storage;

public interface IFileStorage
{
    Task<string> SaveFileAsync(
        Stream fileStream,
        string fileExtension,
        string folderName,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
}