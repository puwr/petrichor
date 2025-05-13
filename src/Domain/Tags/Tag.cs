using Domain.Images;

namespace Domain.Tags;

public class Tag 
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public List<Image> Images { get; private set; } = [];

    public Tag(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    private Tag() {}
}