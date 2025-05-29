using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.UserManagement.Commands.UpdateProfile;

public record UpdateProfileCommand(int UserId, string? FirstName = null, string? LastName = null)
    : IRequest<Result<UserDto>>;
