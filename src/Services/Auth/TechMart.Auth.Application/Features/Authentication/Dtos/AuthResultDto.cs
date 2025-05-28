namespace TechMart.Auth.Application.Features.Authentication.Dtos;

public sealed record AuthResultDto(
    bool IsSuccess,
    string? ErrorMessage = null,
    string? ErrorCode = null
);
