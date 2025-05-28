namespace TechMart.Auth.Application.Features.Shared.Dtos;

public sealed record UserInfoVm(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IEnumerable<string> Roles
)
{
    /// <summary>
    /// Gets the user's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Gets the user's display name (first name or email if no first name)
    /// </summary>
    public string DisplayName => string.IsNullOrWhiteSpace(FirstName) ? Email : FirstName;

    /// <summary>
    /// Converts to ClaimsPrincipal for authorization
    /// </summary>
    public System.Security.Claims.ClaimsPrincipal ToPrincipal()
    {
        var claims = new List<System.Security.Claims.Claim>
        {
            new(System.Security.Claims.ClaimTypes.NameIdentifier, Id.ToString()),
            new(System.Security.Claims.ClaimTypes.Email, Email),
            new(System.Security.Claims.ClaimTypes.Name, FullName),
            new(System.Security.Claims.ClaimTypes.GivenName, FirstName),
            new(System.Security.Claims.ClaimTypes.Surname, LastName),
        };

        foreach (var role in Roles)
        {
            claims.Add(
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)
            );
        }

        return new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(claims, "Bearer")
        );
    }
}
