using ErrorOr;
using MediatR;
using Petrichor.Shared.Contracts.Pagination;
using Petrichor.Modules.Gallery.Contracts.Images;

namespace Petrichor.Modules.Gallery.Application.Images.Queries.ListImages;

public record ListImagesQuery(PaginationParameters Pagination, List<string>? Tags)
    : IRequest<ErrorOr<PagedResponse<ListImagesResponse>>>;