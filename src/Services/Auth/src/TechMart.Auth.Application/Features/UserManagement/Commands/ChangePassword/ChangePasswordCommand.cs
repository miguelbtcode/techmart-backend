using TechMart.Auth.Application.Common.Results;
using MediatR;

namespace TechMart.Auth.Application.Features.UserManagement.Commands.ChangePassword;

public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword)
    : IRequest<Result>;
