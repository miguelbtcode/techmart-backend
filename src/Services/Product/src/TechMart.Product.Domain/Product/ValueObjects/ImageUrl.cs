using System.Text.RegularExpressions;
using TechMart.Product.Domain.Product.Errors;
using TechMart.SharedKernel.Base;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Domain.Product.ValueObjects;

public class ImageUrl : BaseValueObject
{
    private static readonly Regex UrlPattern = new(@"^https?://.*\.(jpg|jpeg|png|gif|webp)(\?.*)?$", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string Value { get; private set; }

    private ImageUrl(string value)
    {
        Value = value;
    }

    public static Result<ImageUrl> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<ImageUrl>(Error.Validation("ImageUrl.Empty", "Image URL cannot be empty"));

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return Result.Failure<ImageUrl>(Error.Validation("ImageUrl.InvalidFormat", "Invalid URL format"));

        if (!UrlPattern.IsMatch(value))
            return Result.Failure<ImageUrl>(ProductErrors.UnsupportedImageFormat(Path.GetExtension(value)));

        return Result.Success(new ImageUrl(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(ImageUrl imageUrl) => imageUrl.Value;
    public override string ToString() => Value;
}