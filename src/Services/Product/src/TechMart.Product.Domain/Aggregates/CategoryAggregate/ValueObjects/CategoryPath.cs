using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Aggregates.CategoryAggregate.ValueObjects;

public class CategoryPath : BaseValueObject
{
    public string Path { get; }
    public int Level { get; }
    public string[] Segments { get; }

    public CategoryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        Path = path.Trim('/');
        Segments = Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        Level = Segments.Length;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Path;
    }

    public static implicit operator string(CategoryPath path) => path.Path;
    public static implicit operator CategoryPath(string path) => new(path);

    public override string ToString() => Path;
}