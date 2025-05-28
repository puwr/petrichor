using Application.Authorization;
using Application.Images.Commands.AddImageTags;
using Application.Images.Commands.DeleteImage;
using Application.Images.Commands.DeleteImageTag;
using Application.Images.Commands.UploadImage;
using Application.Images.Queries.GetImage;
using Application.Images.Queries.ListImages;
using Contracts.Images;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
public class ImagesController(ISender mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
        var uploadImageCommand = new UploadImageCommand(request.ImageFile);

        var uploadImageResult = await mediator.Send(uploadImageCommand);

        return uploadImageResult.Match(
            imageId => CreatedAtAction(nameof(GetImage), new { imageId }, imageId),
            Problem
        );
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ListImages()
    {
        var query = new ListImagesQuery();

        var getImagesResult = await mediator.Send(query);

        return getImagesResult.Match(
            Ok,
            Problem
        );
    }

    [AllowAnonymous]
    [HttpGet("{imageId:guid}")]
    public async Task<IActionResult> GetImage(Guid imageId)
    {
        var query = new GetImageQuery(imageId);

        var getImageResult = await mediator.Send(query);

        return getImageResult.Match(
            Ok,
            Problem
        );
    }

    [Authorize(Policy = AuthorizationPolicies.ImageUploader)]
    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var command = new DeleteImageCommand(imageId);

        var deleteImageResult = await mediator.Send(command);

        return deleteImageResult.Match(
            _ => NoContent(),
            Problem);
    }

    [Authorize(Policy = AuthorizationPolicies.ImageUploader)]
    [HttpPost("{imageId:guid}/tags")]
    public async Task<IActionResult> AddImageTags(Guid imageId, AddTagsRequest request)
    {
        var command = new AddImageTagsCommand(imageId, request.Tags);

        var addImageTagResult = await mediator.Send(command);

        return addImageTagResult.Match(
            success => Ok(),
            Problem
        );
    }

    [Authorize(Policy = AuthorizationPolicies.ImageUploader)]
    [HttpDelete("{imageId:guid}/tags/{tagId:guid}")]
    public async Task<IActionResult> DeleteImageTag(Guid imageId, Guid tagId)
    {
        var command = new DeleteImageTagCommand(imageId, tagId);

        var deleteImageTagResult = await mediator.Send(command);

        return deleteImageTagResult.Match(
            _ => NoContent(),
            Problem);
    }
}