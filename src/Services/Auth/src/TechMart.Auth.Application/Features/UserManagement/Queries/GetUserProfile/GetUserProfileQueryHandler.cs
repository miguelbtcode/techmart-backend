using TechMart.Auth.Application.Common.DTOs;
using TechMart.Auth.Application.Common.Results;
using TechMart.Auth.Domain.Interfaces;
using MediatR;

namespace TechMart.Auth.Application.Features.UserManagement.Queries.GetUserProfile;

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsEmailVerified = user.IsEmailVerified,
            IsTwoFactorEnabled = user.IsTwoFactorEnabled,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
        };

        return Result<UserDto>.Success(userDto);
    }
}
