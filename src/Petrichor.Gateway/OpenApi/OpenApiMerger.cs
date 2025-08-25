using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
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
        document.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();

        var proxyConfig = proxyConfigProvider.GetConfig();

        var mainCluster = proxyConfig.Clusters
            .First(c => c.ClusterId == "petrichor-cluster");

        var commentsCluster = proxyConfig.Clusters
            .First(c => c.ClusterId == "petrichor-comments-cluster");

        var mainUrl = new UriBuilder(mainCluster.Destinations!["destination1"].Address)
        {
            Path = "openapi/v1.json"
        }.Uri;

        var commentsUrl = new UriBuilder(commentsCluster.Destinations!["destination1"].Address)
        {
            Path = "openapi/v1.json"
        }.Uri;

        await MergeOpenApiDocumentAsync(document, mainUrl , cancellationToken);
        await MergeOpenApiDocumentAsync(document, commentsUrl, cancellationToken);
    }

    private async Task MergeOpenApiDocumentAsync(
        OpenApiDocument targetDocument,
        Uri documentUrl,
        CancellationToken cancellationToken)
    {
        using var stream = await httpClient.GetStreamAsync(documentUrl, cancellationToken);

        var document = new OpenApiStreamReader().Read(stream, out var _);

        if (document.Paths is not null)
        {
            foreach (var path in document.Paths)
            {
                targetDocument.Paths.TryAdd(path.Key, path.Value);
            }
        }

        if (document.Components.Schemas is not null)
        {
            foreach (var schema in document.Components.Schemas)
            {
                targetDocument.Components.Schemas.TryAdd(schema.Key, schema.Value);
            }
        }
    }
}