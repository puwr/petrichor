using ErrorOr;
using MediatR;
using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Gallery.Features.GetImages;

public record GetImagesQuery(PaginationParameters Pagination, List<string>? Tags)
    : IRequest<ErrorOr<PagedResponse<GetImagesResponse>>>;