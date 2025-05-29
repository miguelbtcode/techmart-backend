using MediatR;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Application.Common.DTOs;

namespace AuthMicroservice.Application.UserProfile.Queries;

public record GetUserProfileQuery(int UserId) : IRequest<Result<UserDto>>;
