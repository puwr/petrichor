namespace Petrichor.Services.Gallery.Features.UploadImage;

public record UploadImageCommand(IFormFile ImageFile, Guid UploaderId);