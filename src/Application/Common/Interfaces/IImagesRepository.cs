using Domain.Images;

namespace Application.Common.Interfaces;

public interface IImagesRepository
{
    Task AddImageAsync(Image image);
}