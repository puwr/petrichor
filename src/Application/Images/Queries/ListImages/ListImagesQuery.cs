using Contracts.Images;
using Contracts.Pagination;
using ErrorOr;
using MediatR;

namespace Application.Images.Queries.ListImages;

public record ListImagesQuery(PaginationParameters Pagination, List<string>? Tags)
    : IRequest<ErrorOr<PagedResponse<ListImagesResponse>>>;