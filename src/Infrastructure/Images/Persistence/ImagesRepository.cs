using Application.Common.Interfaces;
using Domain.Images;
using Infrastructure.Common.Persistence;

namespace Infrastructure.Images.Persistence;

public class ImagesRepository(PetrichorDbContext dbContext) : IImagesRepository
{
    private readonly PetrichorDbContext _dbContext = dbContext;

    public async Task AddImageAsync(Image image)
    {
        await _dbContext.Images.AddAsync(image);
    }
}