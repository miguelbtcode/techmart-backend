using FluentValidation;

namespace TechMart.Product.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ParentCategoryId)
            .NotEmpty().WithMessage("Parent category ID cannot be empty when provided")
            .When(x => x.ParentCategoryId.HasValue);

        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl).WithMessage("Image URL must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Sort order must be greater than or equal to 0");
    }

    private bool BeValidUrl(string? url)
    {
        return string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}