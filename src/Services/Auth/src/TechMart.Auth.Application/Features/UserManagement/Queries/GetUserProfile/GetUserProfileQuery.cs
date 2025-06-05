using TechMart.Auth.Application.Common.DTOs;
using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.UserManagement.Queries.GetUserProfile;

public record GetUserProfileQuery(int UserId) : IRequest<Result<UserDto>>;
