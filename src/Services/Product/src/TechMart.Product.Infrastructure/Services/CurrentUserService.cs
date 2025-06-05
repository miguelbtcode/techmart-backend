using Microsoft.AspNetCore.Http;
using TechMart.SharedKernel.Abstractions;
using TechMart.SharedKernel.Extensions.Security;

namespace TechMart.Product.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.GetUserId();

    public string? UserName => _httpContextAccessor.HttpContext?.User?.GetUserName();

    public string? Email => _httpContextAccessor.HttpContext?.User?.GetEmail();

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.GetRoles() ?? Enumerable.Empty<string>();

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.IsAuthenticated() ?? false;

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.GetClaimValue(claimType);
    }
}