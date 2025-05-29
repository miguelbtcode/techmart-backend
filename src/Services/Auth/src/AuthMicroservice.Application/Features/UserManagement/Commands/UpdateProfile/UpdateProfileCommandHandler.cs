using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Domain.Interfaces;
using MediatR;

namespace AuthMicroservice.Application.Features.UserManagement.Commands.UpdateProfile;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public UpdateProfileHandler(IUserRepository userRepository, IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<Result<UserDto>> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        // Update profile fields if provided
        if (request.FirstName != null)
            user.FirstName = request.FirstName.Trim();

        if (request.LastName != null)
            user.LastName = request.LastName.Trim();

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        // Publish event
        // await _mediator.Publish(
        //     new UserProfileUpdatedEvent(user.Id, user.Email, DateTime.UtcNow),
        //     cancellationToken
        // );

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

        return Result<UserDto>.Success(userDto, "Profile updated successfully");
    }
}
