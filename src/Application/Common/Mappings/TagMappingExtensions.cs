using Contracts.Tags;
using Domain.Tags;

namespace Application.Common.Mappings;

public static class TagMappingExtensions
{
    public static TagResponse ToResponse(this Tag tag)
    {
        return new TagResponse(Id: tag.Id, Name: tag.Name);
    }
}