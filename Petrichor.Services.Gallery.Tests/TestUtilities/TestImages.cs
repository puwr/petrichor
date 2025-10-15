using System.Reflection;

namespace Petrichor.Services.Gallery.Tests.TestUtilities;

public class TestImages
{
    private static readonly Assembly Assembly = typeof(TestImages).Assembly;
    private const string BasePath = "Petrichor.Services.Gallery.Tests.TestUtilities";

    public static Stream GetImageStream(string fileName)
    {
        var resource = $"{BasePath}.{fileName}";
        var stream = Assembly.GetManifestResourceStream(resource);

        if (stream is null)
        {
            throw new FileNotFoundException($"'{fileName}' not found.");
        }

        return stream;
    }
}