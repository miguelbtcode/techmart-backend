using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.UserManagement.Queries.GetUserProfile;

public record GetUserProfileQuery(int UserId) : IRequest<Result<UserDto>>;
