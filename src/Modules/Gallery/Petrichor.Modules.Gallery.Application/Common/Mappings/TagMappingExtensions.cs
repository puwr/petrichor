using Petrichor.Modules.Gallery.Contracts.Tags;
using Petrichor.Modules.Gallery.Domain.Tags;

namespace Petrichor.Modules.Gallery.Application.Common.Mappings;

public static class TagMappingExtensions
{
    public static TagResponse ToResponse(this Tag tag)
    {
        return new TagResponse(Id: tag.Id, Name: tag.Name);
    }
}