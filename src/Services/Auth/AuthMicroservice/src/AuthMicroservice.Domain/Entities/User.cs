using System.ComponentModel.DataAnnotations;

namespace AuthMicroservice.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsTwoFactorEnabled { get; set; } = false;
    public string? TwoFactorSecret { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } =
        new List<EmailVerificationToken>();
    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } =
        new List<PasswordResetToken>();
    public virtual ICollection<SocialLogin> SocialLogins { get; set; } = new List<SocialLogin>();

    public bool CanResetPassword() => IsActive && IsEmailVerified;

    public bool CanLogin() => IsActive;

    public void MarkEmailAsVerified()
    {
        IsEmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
