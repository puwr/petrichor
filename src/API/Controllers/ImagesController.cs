using Application.Common.Interfaces;
using Application.Images.Commands.AddImageTag;
using Application.Images.Commands.DeleteImage;
using Application.Images.Commands.DeleteImageTag;
using Application.Images.Commands.UploadImage;
using Application.Images.Queries.GetImage;
using Application.Images.Queries.ListImages;
using Contracts.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
public class ImagesController(
    ISender _mediator,
    IUploadsRepository _uploadsRepository,
    IThumbnailsRepository _thumbnailsRepository) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
        var imagePath = await _uploadsRepository.SaveFileAsync(request.Image);
        var thumbnailPath = await _thumbnailsRepository.GenerateAndSaveThumbnail(imagePath);

        var uploadImageCommand = new UploadImageCommand(imagePath, thumbnailPath, Guid.NewGuid());

        var uploadImageResult = await _mediator.Send(uploadImageCommand);

        return uploadImageResult.Match(
            Ok,
            Problem
        );
    }

    [HttpGet]
    public async Task<IActionResult> ListImages()
    {
        var query = new ListImagesQuery();

        var getImagesResult = await _mediator.Send(query);

        return getImagesResult.Match(
            Ok,
            Problem
        );
    }

    [HttpGet("{imageId:guid}")]
    public async Task<IActionResult> GetImage(Guid imageId)
    {
        var query = new GetImageQuery(imageId);

        var getImageResult = await _mediator.Send(query);

        return getImageResult.Match(
            Ok,
            Problem
        );
    }

    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var command = new DeleteImageCommand(imageId);

        var deleteImageResult = await _mediator.Send(command);

        return deleteImageResult.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpPost("{imageId:guid}/tags")]
    public async Task<IActionResult> AddImageTag(Guid imageId, AddTagRequest request)
    {
        var command = new AddImageTagCommand(imageId, request.Tag);

        var addImageTagResult = await _mediator.Send(command);

        return addImageTagResult.Match(
            success => Ok(),
            Problem
        );
    }

    [HttpDelete("{imageId:guid}/tags/{tagId:guid}")]
    public async Task<IActionResult> DeleteImageTag(Guid imageId, Guid tagId)
    {
        var command = new DeleteImageTagCommand(imageId, tagId);

        var deleteImageTagResult = await _mediator.Send(command);

        return deleteImageTagResult.Match(
            _ => NoContent(),
            Problem);
    }
}