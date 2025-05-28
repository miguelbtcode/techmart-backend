using TechMart.Auth.Application.Contracts.Context;

namespace TechMart.Auth.Infrastructure.Authentication;

/// <summary>
/// Thread-safe current user context using AsyncLocal
/// Allows setting user context without HTTP dependencies
/// </summary>
public sealed class CurrentUserContext : ICurrentUserContext
{
    private static readonly AsyncLocal<CurrentUserInfo?> _currentUser = new();

    public Guid? UserId => _currentUser.Value?.UserId;
    public string? UserEmail => _currentUser.Value?.Email;
    public string? UserName => _currentUser.Value?.UserName;
    public IReadOnlyList<string> UserRoles => _currentUser.Value?.Roles ?? Array.Empty<string>();
    public bool IsAuthenticated => _currentUser.Value != null;
    public string? IpAddress => _currentUser.Value?.IpAddress;
    public string? UserAgent => _currentUser.Value?.UserAgent;

    public void SetCurrentUser(CurrentUserInfo userInfo)
    {
        _currentUser.Value = userInfo ?? throw new ArgumentNullException(nameof(userInfo));
    }

    public void Clear()
    {
        _currentUser.Value = null;
    }
}
