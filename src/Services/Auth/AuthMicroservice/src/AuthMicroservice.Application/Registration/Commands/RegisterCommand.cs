using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Registration.Commands;

public record RegisterCommand(
    string Email,
    string Username,
    string Password,
    string? FirstName = null,
    string? LastName = null
) : IRequest<Result<UserDto>>;
