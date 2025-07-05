using System.Reflection;

namespace TestUtilities.Images;

public class TestImages
{
    private static readonly Assembly Assembly = typeof(TestImages).Assembly;
    private const string BasePath = "TestUtilities.Images";

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
