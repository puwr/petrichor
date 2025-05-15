using Application.Common.Interfaces;
using Domain.Images;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Images.Persistence;

public class ImagesRepository(PetrichorDbContext dbContext) : IImagesRepository
{
    private readonly PetrichorDbContext _dbContext = dbContext;

    public async Task AddImageAsync(Image image)
    {
        await _dbContext.Images.AddAsync(image);
    }

    public async Task<List<Image>> ListAsync()
    {
        return await _dbContext.Images.Include(i => i.Tags).ToListAsync();
    }

    public async Task<Image?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Images.Include(i => i.Tags).FirstOrDefaultAsync(image => image.Id == id);
    }

    public Task UpdateImageAsync(Image image)
    {
        _dbContext.Update(image);

        return Task.CompletedTask;
    }

    public Task RemoveImageAsync(Image image)
    {
        _dbContext.Remove(image);

        return Task.CompletedTask;
    }
}