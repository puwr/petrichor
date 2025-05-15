using Application.Common.Interfaces;
using Domain.Tags;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tags.Persistence;

public class TagsRepository(PetrichorDbContext dbContext) : ITagsRepository
{
    private readonly PetrichorDbContext _dbContext = dbContext;

    public async Task<Tag?> GetByNameAsync(string tagName)
    {
        return await _dbContext.Tags.Where(t => t.Name == tagName).FirstOrDefaultAsync();
    }
}