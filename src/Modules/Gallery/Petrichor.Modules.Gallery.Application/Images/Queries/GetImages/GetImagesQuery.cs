using ErrorOr;
using MediatR;
using Petrichor.Modules.Gallery.Contracts.Images;
using Petrichor.Shared.Pagination;

namespace Petrichor.Modules.Gallery.Application.Images.Queries.GetImages;

public record GetImagesQuery(PaginationParameters Pagination, List<string>? Tags)
    : IRequest<ErrorOr<PagedResponse<GetImagesResponse>>>;