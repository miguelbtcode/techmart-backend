using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.UserProfile.Commands;

public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword)
    : IRequest<Result>;
