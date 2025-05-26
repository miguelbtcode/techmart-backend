using FluentValidation;
using TechMart.Auth.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersQueryValidator()
    {
        RuleFor(x => x.Pagination.PageIndex)
            .GreaterThan(0)
            .WithMessage("Page index must be greater than 0.");

        RuleFor(x => x.Pagination.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");

        RuleFor(x => x.Pagination.SortBy)
            .Must(BeAValidSortField)
            .When(x => !string.IsNullOrWhiteSpace(x.Pagination.SortBy))
            .WithMessage(
                "Invalid sort field. Allowed: email, firstname, lastname, lastloginat, createdat."
            );

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Invalid user status.");
    }

    private bool BeAValidSortField(string? sortBy)
    {
        var validFields = new[] { "email", "firstname", "lastname", "lastloginat", "createdat" };
        return validFields.Contains(sortBy!.ToLower());
    }
}
