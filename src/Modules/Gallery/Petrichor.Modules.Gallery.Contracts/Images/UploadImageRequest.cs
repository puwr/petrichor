using Microsoft.AspNetCore.Http;

namespace Petrichor.Modules.Gallery.Contracts.Images;

public record UploadImageRequest(IFormFile ImageFile);