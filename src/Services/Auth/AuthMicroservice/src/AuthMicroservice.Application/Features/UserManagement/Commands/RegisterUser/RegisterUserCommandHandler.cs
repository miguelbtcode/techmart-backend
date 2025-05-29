using AuthMicroservice.Application.Common.DTOs;
using AuthMicroservice.Application.Common.Results;
using AuthMicroservice.Domain.Entities;
using AuthMicroservice.Domain.Events;
using AuthMicroservice.Domain.Interfaces;
using BCrypt.Net;
using MediatR;

namespace AuthMicroservice.Application.Features.UserManagement.Commands.RegisterUser;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public RegisterHandler(IUserRepository userRepository, IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<Result<UserDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        // Check if email exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return Result<UserDto>.Failure("Email already exists");
        }

        // Check if username exists
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            return Result<UserDto>.Failure("Username already exists");
        }

        // Create user
        var user = new User
        {
            Email = request.Email.ToLowerInvariant().Trim(),
            Username = request.Username.ToLowerInvariant().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        var createdUser = await _userRepository.CreateAsync(user);

        // Publish event
        await _mediator.Publish(
            new UserRegisteredEvent(
                createdUser.Id,
                createdUser.Email,
                createdUser.Username,
                DateTime.UtcNow
            ),
            cancellationToken
        );

        var userDto = new UserDto
        {
            Id = createdUser.Id,
            Email = createdUser.Email,
            Username = createdUser.Username,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            IsEmailVerified = createdUser.IsEmailVerified,
            IsTwoFactorEnabled = createdUser.IsTwoFactorEnabled,
            CreatedAt = createdUser.CreatedAt,
        };

        return Result<UserDto>.Success(userDto, "User registered successfully");
    }
}
