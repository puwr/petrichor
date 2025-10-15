using Petrichor.Services.Gallery.Common.Domain.Images;

namespace Petrichor.Services.Gallery.Common.Domain;

public sealed class Tag
{
    private readonly List<Image> _images = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyCollection<Image> Images => _images.AsReadOnly();

    public static Tag Create(string name)
    {
        return new Tag
        {
            Id = Guid.CreateVersion7(),
            Name = name
        };
    }
}