using Petrichor.Shared.Infrastructure.Services.Storage;

namespace Petrichor.Shared.Infrastructure.Tests.Services.Storage;

[Trait("Category", "Integration")]
public class LocalFileStorageTests : IDisposable
{
    private readonly string _testDataFolder;
    private readonly LocalFileStorage _storage;

    public LocalFileStorageTests()
    {
        _testDataFolder = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}");

        Directory.CreateDirectory(_testDataFolder);

        _storage = new(_testDataFolder);
    }

    [Fact]
    public async Task SaveFileAsync_SavesFile()
    {
        var testData = new byte[1];
        using var testFileStream = new MemoryStream(testData);
        var filePath = await _storage.SaveFileAsync(testFileStream, ".test", "test");
        var sanitizedFilePath = Path.Combine(filePath.Split("/"));

        var fullPath = Path.Combine(_testDataFolder, sanitizedFilePath);
        File.Exists(fullPath).Should().BeTrue();

        var savedFileBytes = await File.ReadAllBytesAsync(fullPath);
        savedFileBytes.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public async Task DeleteFileAsync_DeletesFile()
    {
        using var testFileStream = new MemoryStream(new byte[1]);
        var filePath = await _storage.SaveFileAsync(testFileStream, ".test", "test");
        var sanitizedFilePath = Path.Combine(filePath.Split("/"));

        var fullPath = Path.Combine(_testDataFolder, sanitizedFilePath);
        File.Exists(fullPath).Should().BeTrue();

        await _storage.DeleteFileAsync(filePath);

        File.Exists(fullPath).Should().BeFalse();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDataFolder))
        {
            Directory.Delete(_testDataFolder, recursive: true);
        }
    }
}