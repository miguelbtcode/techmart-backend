namespace TechMart.Auth.Domain.Entities;

public class SocialLogin
{
    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderUserId { get; set; } = string.Empty;
    public string? ProviderEmail { get; set; }
    public string? ProviderName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public void UpdateLastUsed()
    {
        LastUsedAt = DateTime.UtcNow;
    }
}
