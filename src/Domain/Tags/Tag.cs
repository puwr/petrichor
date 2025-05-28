using Domain.Images;

namespace Domain.Tags;

public sealed class Tag 
{
    private readonly List<Image> _images = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyCollection<Image> Images => _images.AsReadOnly();

    public Tag(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    private Tag() { }
}