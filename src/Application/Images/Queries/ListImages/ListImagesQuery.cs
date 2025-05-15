using Domain.Images;
using ErrorOr;
using MediatR;

namespace Application.Images.Queries.ListImages;

public record ListImagesQuery() : IRequest<ErrorOr<List<Image>>>;