using Contracts.Images;
using Contracts.Pagination;
using ErrorOr;
using MediatR;

namespace Application.Images.Queries.ListImages;

public record ListImagesQuery(PaginationParameters Pagination)
    : IRequest<ErrorOr<PagedResponse<ListImagesResponse>>>;