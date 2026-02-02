using System.Reflection;

namespace Petrichor.Services.Gallery.Tests.TestUtilities;

public static class ApiClientExtensions
{
    public static async Task<HttpResponseMessage> UploadTestImageAsync(this HttpClient client)
    {
        Assembly assembly = typeof(ApiClientExtensions).Assembly;
        const string resourceName = "Petrichor.Services.Gallery.Tests.TestUtilities.test-image.jpg";
        await using var imageStream = assembly.GetManifestResourceStream(resourceName);

        if (imageStream is null)
        {
            throw new FileNotFoundException($"Test image not found.");
        }

        using var formData = new MultipartFormDataContent
        {
            { new StreamContent(imageStream), "ImageFile", "test.jpg" }
        };

        return await client.PostAsync("/images", formData);
    }
}