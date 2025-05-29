using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.UserManagement.Commands.RegisterUser;

public record RegisterCommand(
    string Email,
    string Username,
    string Password,
    string? FirstName = null,
    string? LastName = null
) : IRequest<Result<UserDto>>;
