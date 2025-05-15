using Application.Common.Interfaces;
using Application.Images.Commands.UploadImage;
using Contracts.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
public class ImagesController(
    ISender _mediator,
    IUploadsRepository _uploadsRepository,
    IThumbnailsRepository _thumbnailsRepository ) : ApiController
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
}