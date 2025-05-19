using Domain.Images;

namespace Application.Common.Interfaces;

public interface IImagesRepository
{
    Task AddImageAsync(Image image);
    Task<List<Image>> ListAsync();
    Task<Image?> GetByIdAsync(Guid id);
    Task UpdateImageAsync(Image image);
    Task RemoveImageAsync(Image image);
    Task<Guid?> GetUploaderIdAsync(Guid id);
}