using Microsoft.AspNetCore.Http;

namespace Contracts.Images;

public record UploadImageRequest(IFormFile ImageFile);