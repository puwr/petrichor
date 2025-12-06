using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Gallery.Features.GetImages;

public record GetImagesQuery(PaginationParameters Pagination, string[]? Tags, string? Uploader);