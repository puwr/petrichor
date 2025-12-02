using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Petrichor.Shared;
using Wolverine;

namespace Petrichor.Services.Gallery.Features.UploadImage;

public class UploadImageEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "images",
            async ([FromForm] UploadImageRequest request, IMessageBus bus, HttpContext httpContext) =>
            {
                var currentUserIdClaim = httpContext.User
                    .FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!Guid.TryParse(currentUserIdClaim, out Guid uploaderId))
                {
                    return Problem(Error.Unauthorized());
                }

                var uploadImageCommand = new UploadImageCommand(
                    ImageFile: request.ImageFile,
                    UploaderId: uploaderId);

                var uploadImageResult = await bus.InvokeAsync<ErrorOr<Guid>>(uploadImageCommand);

                return uploadImageResult.Match(
                    imageId =>
                    {
                        var path = $"/images/{imageId}";
                        return Results.Created(path, imageId);
                    },
                    Problem
                );
            })
            .RequireAuthorization()
            .WithTags(Tags.Images)
            .WithSummary("Upload image")
            .Accepts<UploadImageRequest>("multipart/form-data")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .DisableAntiforgery();
    }
}