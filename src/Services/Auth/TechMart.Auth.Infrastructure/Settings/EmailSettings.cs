namespace TechMart.Auth.Infrastructure.Services.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string ApiKey { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
    public string FromName { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = string.Empty;
    public bool EnableEmailSending { get; init; } = true;
}
