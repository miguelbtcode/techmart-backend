namespace AuthMicroservice.Models.DTOs;

public class LoginRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}
