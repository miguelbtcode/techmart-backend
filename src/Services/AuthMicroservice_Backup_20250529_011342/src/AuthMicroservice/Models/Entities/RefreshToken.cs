using System.ComponentModel.DataAnnotations;

namespace AuthMicroservice.Models.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;

    // Foreign Key
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
