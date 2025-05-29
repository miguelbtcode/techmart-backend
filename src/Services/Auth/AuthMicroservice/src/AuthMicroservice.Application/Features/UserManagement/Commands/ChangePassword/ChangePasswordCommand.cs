using AuthMicroservice.Application.Common.Results;
using MediatR;

namespace AuthMicroservice.Application.Features.UserManagement.Commands.ChangePassword;

public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword)
    : IRequest<Result>;
