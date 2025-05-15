using Application.Common.Interfaces;
using Infrastructure.Common.Persistence;
using Infrastructure.Images.Persistence;
using Infrastructure.Tags.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<PetrichorDbContext>(options =>
            options.UseSqlite("Data Source = Petrichor.db"));

        services.AddScoped<IImagesRepository, ImagesRepository>();
        services.AddScoped<ITagsRepository, TagsRepository>();
        services.AddScoped<IUploadsRepository, UploadsRepository>();
        services.AddScoped<IThumbnailsRepository, ThumbnailsRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<PetrichorDbContext>());

        return services;
    }
}