using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Modules.Shared.Presentation;
using Petrichor.Modules.Gallery.Application.Images.Commands.AddImageTags;
using Petrichor.Modules.Gallery.Application.Images.Commands.DeleteImage;
using Petrichor.Modules.Gallery.Application.Images.Commands.DeleteImageTag;
using Petrichor.Modules.Gallery.Application.Images.Commands.UploadImage;
using Petrichor.Modules.Gallery.Application.Images.Queries.GetImage;
using Petrichor.Modules.Gallery.Application.Images.Queries.GetImages;
using Petrichor.Modules.Gallery.Contracts.Images;
using Petrichor.Modules.Gallery.Infrastructure.Authorization;
using Petrichor.Shared.Pagination;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Petrichor.Modules.Gallery.Presentation.Controllers;

public class ImagesController(ISender mediator) : ApiController
{
    [HttpPost]
    [EndpointSummary("Upload image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
        var currentUserIdClaim = User
            .FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(currentUserIdClaim, out Guid uploaderId))
        {
            return Problem(Error.Unauthorized());
        }

        var uploadImageCommand = new UploadImageCommand(
            ImageFile: request.ImageFile,
            UploaderId: uploaderId);

        var uploadImageResult = await mediator.Send(uploadImageCommand);

        return uploadImageResult.Match(
            imageId =>
            {
                var path = $"/images/{imageId}";
                return Created(path, imageId);
            },
            Problem
        );
    }

    [AllowAnonymous]
    [HttpGet]
    [EndpointSummary("Get images")]
    [ProducesResponseType<PagedResponse<GetImagesResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImages(
        [FromQuery(Name = "page")] int pageNumber = 1,
        [FromQuery] List<string>? tags = null)
    {
        var pagination = new PaginationParameters(pageNumber, PageSize: 14);

        var query = new GetImagesQuery(pagination, tags);

        var getImagesResult = await mediator.Send(query);

        return getImagesResult.Match(
            Ok,
            Problem
        );
    }

    [AllowAnonymous]
    [HttpGet("{imageId:guid}")]
    [EndpointSummary("Get image")]
    [ProducesResponseType<ImageResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(Guid imageId)
    {
        var query = new GetImageQuery(imageId);

        var getImageResult = await mediator.Send(query);

        return getImageResult.Match(
            Ok,
            Problem
        );
    }

    [Authorize(Policy = GalleryPolicies.ImageUploaderOrAdmin)]
    [HttpDelete("{imageId:guid}")]
    [EndpointSummary("Delete image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var command = new DeleteImageCommand(imageId);

        var deleteImageResult = await mediator.Send(command);

        return deleteImageResult.Match(
            _ => NoContent(),
            Problem);
    }

    [Authorize(Policy = GalleryPolicies.ImageUploaderOrAdmin)]
    [HttpPost("{imageId:guid}/tags")]
    [EndpointSummary("Add tags to image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddImageTags(Guid imageId, AddTagsRequest request)
    {
        var command = new AddImageTagsCommand(imageId, request.Tags);

        var addImageTagResult = await mediator.Send(command);

        return addImageTagResult.Match(
            _ => NoContent(),
            Problem
        );
    }

    [Authorize(Policy = GalleryPolicies.ImageUploaderOrAdmin)]
    [HttpDelete("{imageId:guid}/tags/{tagId:guid}")]
    [EndpointSummary("Delete tag from image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteImageTag(Guid imageId, Guid tagId)
    {
        var command = new DeleteImageTagCommand(imageId, tagId);

        var deleteImageTagResult = await mediator.Send(command);

        return deleteImageTagResult.Match(
            _ => NoContent(),
            Problem);
    }
}