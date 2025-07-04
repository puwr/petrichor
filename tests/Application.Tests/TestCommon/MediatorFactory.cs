using API;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TestUtilities.Database;

namespace Application.Tests.TestCommon;

public class MediatorFactory: IDisposable
{
    private readonly TestDatabase _testDatabase = TestDatabase.CreateDatabase();
    private readonly WebApplicationFactory<IApiMarker> _webApplicationFactory;
    private IServiceScope _scope;

    public MediatorFactory(Action<IServiceCollection>? defaultServices = null)
    {
        _webApplicationFactory = new WebApplicationFactory<IApiMarker>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services
                        .RemoveAll<DbContextOptions<PetrichorDbContext>>()
                        .AddDbContext<PetrichorDbContext>(
                            (options) => options.UseSqlite(_testDatabase.Connection));

                    defaultServices?.Invoke(services);
                });
            });
    }

    public IMediator CreateMediator(Action<IServiceCollection>? services = null)
    {
        _scope?.Dispose();
        _testDatabase.ResetDatabase();

        var factory = services is not null
            ? _webApplicationFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services);
            })
            : _webApplicationFactory;

        _scope = factory.Services.CreateScope();
        return _scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public void Dispose()
    {
        _testDatabase.Dispose();
        _webApplicationFactory.Dispose();
        _scope.Dispose();
    }
}
