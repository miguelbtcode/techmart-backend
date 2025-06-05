using FluentValidation;

namespace TechMart.Product.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required")
            .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.LogoUrl)
            .Must(BeValidUrl).WithMessage("Logo URL must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.LogoUrl));

        RuleFor(x => x.WebsiteUrl)
            .Must(BeValidUrl).WithMessage("Website URL must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.WebsiteUrl));
    }

    private bool BeValidUrl(string? url)
    {
        return string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}