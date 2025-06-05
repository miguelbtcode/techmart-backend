namespace TechMart.Auth.Domain.Interfaces;

public record SocialUserInfo(
    string Id,
    string Email,
    string Name,
    string? AvatarUrl = null
);

public interface ISocialAuthService
{
    Task<SocialUserInfo?> GetGoogleUserInfoAsync(string accessToken);
    Task<SocialUserInfo?> GetGitHubUserInfoAsync(string accessToken);
    Task<string> GetGoogleAuthUrlAsync(string redirectUri);
    Task<string> GetGitHubAuthUrlAsync(string redirectUri);
}
