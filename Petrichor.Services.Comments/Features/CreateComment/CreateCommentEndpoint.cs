using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErrorOr;
using MediatR;
using Petrichor.Shared.Features;

namespace Petrichor.Services.Comments.Features.CreateComment;

public class CreateCommentEndpoint : FeatureEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost(
            "comments",
            async (CreateCommentRequest request, ISender mediator, HttpContext httpContext) =>
            {
                var currentUserIdClaim = httpContext.User
                    .FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (!Guid.TryParse(currentUserIdClaim, out Guid authorId))
                {
                    return Problem(Error.Unauthorized());
                }

                var command = new CreateCommentCommand(
                    authorId,
                    request.ResourceId,
                    request.Message);

                var createCommentResult = await mediator.Send(command);

                return createCommentResult.Match(
                    commentId => Results.Created($"/comments?resourceId={request.ResourceId}", commentId),
                    Problem
                );
            })
            .RequireAuthorization()
            .WithTags(Tags.Comments)
            .WithSummary("Create comment")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}