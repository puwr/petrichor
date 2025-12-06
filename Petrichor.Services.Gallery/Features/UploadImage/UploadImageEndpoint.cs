using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            async (
                [FromForm] UploadImageRequest request,
                IMessageBus bus,
                ClaimsPrincipal user,
                CancellationToken cancellationToken) =>
            {
                var uploaderId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

                var uploadImageCommand = new UploadImageCommand(
                    ImageFile: request.ImageFile,
                    UploaderId: uploaderId);

                var uploadedImageId = await bus
                    .InvokeAsync<Guid>(uploadImageCommand, cancellationToken);

                return Results.Created($"/images/{uploadedImageId}", uploadedImageId);
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