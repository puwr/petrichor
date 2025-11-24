using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Yarp.ReverseProxy.Configuration;

namespace Petrichor.Gateway.OpenApi;

public class OpenApiMerger(
    HttpClient httpClient,
    IProxyConfigProvider proxyConfigProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();

        var proxyConfig = proxyConfigProvider.GetConfig();

        var usersCluster = proxyConfig.Clusters
            .First(c => c.ClusterId == "petrichor-users-cluster");

        var galleryCluster = proxyConfig.Clusters
            .First(c => c.ClusterId == "petrichor-gallery-cluster");

        var commentsCluster = proxyConfig.Clusters
            .First(c => c.ClusterId == "petrichor-comments-cluster");

        var usersUrl = new UriBuilder(usersCluster.Destinations!["destination1"].Address)
        {
            Path = "openapi/v1.json"
        }.Uri;

        var galleryUrl = new UriBuilder(galleryCluster.Destinations!["destination1"].Address)
        {
            Path = "openapi/v1.json"
        }.Uri;

        var commentsUrl = new UriBuilder(commentsCluster.Destinations!["destination1"].Address)
        {
            Path = "openapi/v1.json"
        }.Uri;


        await MergeOpenApiDocumentAsync(document, usersUrl, cancellationToken);
        await MergeOpenApiDocumentAsync(document, galleryUrl, cancellationToken);
        await MergeOpenApiDocumentAsync(document, commentsUrl, cancellationToken);
    }

    private async Task MergeOpenApiDocumentAsync(
        OpenApiDocument targetDocument,
        Uri documentUrl,
        CancellationToken cancellationToken)
    {
        var jsonBytes = await httpClient.GetByteArrayAsync(documentUrl, cancellationToken);
        using var memoryStream = new MemoryStream(jsonBytes);

        var (document, diagnostic) = await OpenApiDocument
            .LoadAsync(memoryStream, cancellationToken: cancellationToken);

        if (diagnostic?.Errors.Any() == true)
        {
            Console.Error.WriteLine($"Failed to load OpenAPI document from {documentUrl}");
        }

        if (document?.Paths?.Count > 0)
        {
            foreach (var path in document.Paths)
            {
                targetDocument.Paths.TryAdd($"api{path.Key}", path.Value);
            }
        }

        if (document?.Components?.Schemas?.Count > 0)
        {
            foreach (var schema in document.Components.Schemas)
            {
                targetDocument?.Components?.Schemas?.TryAdd(schema.Key, schema.Value);
            }
        }
    }
}