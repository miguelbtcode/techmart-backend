namespace TechMart.Auth.Application.Features.Users.Vms;

public sealed record TokenValidationVm(bool IsValid, Guid? UserId, DateTime? ExpiresAt);
