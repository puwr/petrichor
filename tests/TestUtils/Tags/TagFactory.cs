using Domain.Tags;
using TestUtils.TestConstants;

namespace TestUtils.Tags;

public static class TagFactory
{
    public static Tag CreateTag()
    {
        return new Tag(Constants.Tag.Name);
    }
}