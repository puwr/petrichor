using Domain.Tags;
using TestUtilities.TestConstants;

namespace TestUtilities.Tags;

public static class TagFactory
{
    public static List<Tag> CreateTags(int count = 1)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Tag($"{Constants.Tag.Name}{i}"))
            .ToList();
    }
}