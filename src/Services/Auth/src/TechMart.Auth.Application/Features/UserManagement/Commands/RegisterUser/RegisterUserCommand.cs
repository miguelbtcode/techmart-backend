using TechMart.Auth.Application.Common.DTOs;
using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.UserManagement.Commands.RegisterUser;

public record RegisterCommand(
    string Email,
    string Username,
    string Password,
    string? FirstName = null,
    string? LastName = null
) : IRequest<Result<UserDto>>;
