using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Petrichor.Shared;

namespace Petrichor.Services.Users.Common.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();

        ServiceDescriptor[] serviceDescriptors = currentAssembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(FeatureEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(FeatureEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        IEnumerable<FeatureEndpoint> endpoints = endpointRouteBuilder
            .ServiceProvider
            .GetRequiredService<IEnumerable<FeatureEndpoint>>();

        foreach (FeatureEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(endpointRouteBuilder);
        }

        return endpointRouteBuilder;
    }
}