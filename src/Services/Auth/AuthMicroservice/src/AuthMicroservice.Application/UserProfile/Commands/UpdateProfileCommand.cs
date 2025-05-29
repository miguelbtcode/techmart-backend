using MediatR;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.Common.DTOs;

namespace AuthMicroservice.Application.UserProfile.Commands;

public record UpdateProfileCommand(
    int UserId,
    string? FirstName = null,
    string? LastName = null
) : IRequest<Result<UserDto>>;
