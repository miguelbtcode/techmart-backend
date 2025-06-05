using TechMart.Auth.Application.Common.DTOs;
using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.UserManagement.Commands.UpdateProfile;

public record UpdateProfileCommand(int UserId, string? FirstName = null, string? LastName = null)
    : IRequest<Result<UserDto>>;
