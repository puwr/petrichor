using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

public interface IUploadsRepository
{
    Task<string> SaveFileAsync(IFormFile file);
    Task RemoveFileAsync(string imagePath);
}