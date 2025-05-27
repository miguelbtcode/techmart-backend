using System.ComponentModel.DataAnnotations;
using TechMart.Auth.API.Controllers.Base;

namespace TechMart.Auth.API.Filters;

/// <summary>
/// Validation filter for endpoints
/// </summary>
public class ValidationFilter<T> : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request is null)
            return await next(context);

        var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(
            request,
            validationContext,
            validationResults,
            true
        );

        if (!isValid)
        {
            var errors = validationResults
                .GroupBy(x => x.MemberNames.FirstOrDefault() ?? "General")
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage ?? "Validation error").ToArray()
                );

            return Results.BadRequest(ApiResponse.ValidationError(errors));
        }

        return await next(context);
    }
}
