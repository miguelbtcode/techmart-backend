using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Contracts.Authentication;
using TechMart.Auth.Application.Contracts.Context;
using TechMart.Auth.Application.Features.Shared.Dtos;
using TechMart.Auth.Domain.Primitives;
using TechMart.Auth.Domain.Users.ValueObjects;

namespace TechMart.Auth.Infrastructure.Authentication;

/// <summary>
/// Service for accessing current user information using context
/// No HTTP dependencies, can be used in any layer
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly ICurrentUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        ICurrentUserContext userContext,
        IUnitOfWork unitOfWork,
        ILogger<CurrentUserService> logger
    )
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public bool IsAuthenticated => _userContext.IsAuthenticated;
    public string? CurrentUserEmail => _userContext.UserEmail;

    public async Task<Guid?> GetCurrentUserIdAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_userContext.UserId);
    }

    public async Task<UserInfoVm?> GetCurrentUserAsync(
        CancellationToken cancellationToken = default
    )
    {
        var userId = _userContext.UserId;
        if (userId == null)
        {
            return null;
        }

        try
        {
            var userIdValue = UserId.From(userId.Value);
            var user = await _unitOfWork.Users.GetByIdWithRolesAsync(
                userIdValue,
                cancellationToken
            );

            if (user == null)
            {
                _logger.LogWarning("Current user {UserId} not found in database", userId);
                return null;
            }

            var roles = user.GetRoleNames();

            return new UserInfoVm(
                user.Id.Value,
                user.Email.Value,
                user.FirstName,
                user.LastName,
                roles
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get current user information for user {UserId}",
                userId
            );
            return null;
        }
    }

    public async Task<IReadOnlyList<string>> GetCurrentUserRolesAsync(
        CancellationToken cancellationToken = default
    )
    {
        var contextRoles = _userContext.UserRoles;
        if (contextRoles.Any())
        {
            return contextRoles;
        }

        // Fallback to database if context doesn't have roles
        var userInfo = await GetCurrentUserAsync(cancellationToken);
        return userInfo != null ? userInfo.Roles.ToList().AsReadOnly() : Array.Empty<string>();
    }

    public async Task<bool> IsInRoleAsync(
        string role,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        var roles = await GetCurrentUserRolesAsync(cancellationToken);
        return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<bool> IsInAnyRoleAsync(
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default
    )
    {
        if (roles == null || !roles.Any())
        {
            return false;
        }

        var userRoles = await GetCurrentUserRolesAsync(cancellationToken);
        return roles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }

    public string? GetCurrentUserIpAddress()
    {
        return _userContext.IpAddress;
    }

    public string? GetCurrentUserAgent()
    {
        return _userContext.UserAgent;
    }
}
