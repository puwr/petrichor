using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Petrichor.Modules.Gallery.Application.Common.Interfaces;
using Petrichor.Modules.Gallery.Infrastructure.Persistence;
using Petrichor.Shared.Behaviors;

namespace Petrichor.Modules.Gallery.Application.Tests;

public class MediatorFactory : IDisposable
{
    private IServiceScope _scope;
    private static readonly ServiceCollection DefaultServices = [];

    public MediatorFactory(Action<IServiceCollection>? testServices = null)
    {
        DefaultServices
            .AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(AssemblyMarker.Assembly);

                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
        DefaultServices.AddValidatorsFromAssembly(AssemblyMarker.Assembly);

        DefaultServices.AddDbContext<GalleryDbContext>(options =>
            options.UseInMemoryDatabase($"{Guid.CreateVersion7()}"));
        DefaultServices.AddScoped<IGalleryDbContext>(provider => provider.GetRequiredService<GalleryDbContext>());

        testServices?.Invoke(DefaultServices);
    }

    public IMediator Create(Action<IServiceCollection>? testServices = null)
    {
        var services = new ServiceCollection();

        foreach (var defaultService in DefaultServices)
        {
            services.Add(defaultService);
        }

        testServices?.Invoke(services);

        _scope = services.BuildServiceProvider().CreateScope();

        return _scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
