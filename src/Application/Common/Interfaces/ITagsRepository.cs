using Domain.Tags;

namespace Application.Common.Interfaces;

public interface ITagsRepository
{
    Task<Tag?> GetByNameAsync(string tagName);
}