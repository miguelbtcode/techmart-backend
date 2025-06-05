using System.Text.Json;
using AuthMicroservice.Domain.Interfaces;

namespace AuthMicroservice.Infrastructure.ExternalServices;

public class SocialAuthService : ISocialAuthService
{
    private readonly HttpClient _httpClient;
    private readonly SocialAuthSettings _settings;

    public SocialAuthService(HttpClient httpClient, SocialAuthSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<SocialUserInfo?> GetGoogleUserInfoAsync(string accessToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                "https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}"
            );

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var googleUser = JsonSerializer.Deserialize<GoogleUserResponse>(json);

            if (googleUser == null)
                return null;

            return new SocialUserInfo(
                googleUser.id,
                googleUser.email,
                googleUser.name,
                googleUser.picture
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task<SocialUserInfo?> GetGitHubUserInfoAsync(string accessToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "token {accessToken}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "$PROJECT_NAME");

            var response = await _httpClient.GetAsync("https://api.github.com/user");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var githubUser = JsonSerializer.Deserialize<GitHubUserResponse>(json);

            if (githubUser == null)
                return null;

            // Get email if not public
            var email = githubUser.email;
            if (string.IsNullOrEmpty(email))
            {
                var emailResponse = await _httpClient.GetAsync(
                    "https://api.github.com/user/emails"
                );
                if (emailResponse.IsSuccessStatusCode)
                {
                    var emailJson = await emailResponse.Content.ReadAsStringAsync();
                    var emails = JsonSerializer.Deserialize<GitHubEmailResponse[]>(emailJson);
                    email = emails?.FirstOrDefault(e => e.primary)?.email;
                }
            }

            return new SocialUserInfo(
                githubUser.id.ToString(),
                email ?? string.Empty,
                githubUser.name ?? githubUser.login,
                githubUser.avatar_url
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> GetGoogleAuthUrlAsync(string redirectUri)
    {
        var scope = "openid%20profile%20email";
        return "https://accounts.google.com/o/oauth2/v2/auth?client_id={_settings.GoogleClientId}&redirect_uri={redirectUri}&scope={scope}&response_type=code";
    }

    public async Task<string> GetGitHubAuthUrlAsync(string redirectUri)
    {
        var scope = "user:email";
        return "https://github.com/login/oauth/authorize?client_id={_settings.GitHubClientId}&redirect_uri={redirectUri}&scope={scope}";
    }
}

public class SocialAuthSettings
{
    public string GoogleClientId { get; set; } = string.Empty;
    public string GoogleClientSecret { get; set; } = string.Empty;
    public string GitHubClientId { get; set; } = string.Empty;
    public string GitHubClientSecret { get; set; } = string.Empty;
}

// Response DTOs
public class GoogleUserResponse
{
    public string id { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string picture { get; set; } = string.Empty;
}

public class GitHubUserResponse
{
    public int id { get; set; }
    public string login { get; set; } = string.Empty;
    public string? name { get; set; }
    public string? email { get; set; }
    public string avatar_url { get; set; } = string.Empty;
}

public class GitHubEmailResponse
{
    public string email { get; set; } = string.Empty;
    public bool primary { get; set; }
}
