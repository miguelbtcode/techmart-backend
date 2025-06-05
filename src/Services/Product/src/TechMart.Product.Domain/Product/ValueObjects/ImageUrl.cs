using System.Text.RegularExpressions;
using TechMart.SharedKernel.Base;

namespace TechMart.Product.Domain.Product.ValueObjects;

public class ImageUrl : BaseValueObject
{
    private static readonly Regex UrlPattern = new(@"^https?://.*\.(jpg|jpeg|png|gif|webp)(\?.*)?$", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string Value { get; }

    public ImageUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Image URL cannot be empty", nameof(value));

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format", nameof(value));

        if (!UrlPattern.IsMatch(value))
            throw new ArgumentException("URL must point to a valid image file (jpg, jpeg, png, gif, webp)", nameof(value));

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public Uri ToUri() => new(Value);

    public string GetFileExtension()
    {
        var uri = ToUri();
        var path = uri.AbsolutePath;
        var extension = Path.GetExtension(path);
        return extension.TrimStart('.');
    }

    public bool IsSecure => Value.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    public static implicit operator string(ImageUrl imageUrl) => imageUrl.Value;
    public static implicit operator ImageUrl(string value) => new(value);

    public override string ToString() => Value;
}