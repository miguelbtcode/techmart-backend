using TechMart.Auth.Application.Features.Users.Vms;
using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserDetailVm>;
