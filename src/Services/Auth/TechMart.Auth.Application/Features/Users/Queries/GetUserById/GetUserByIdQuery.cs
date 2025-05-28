using TechMart.Auth.Application.Messaging.Queries;

namespace TechMart.Auth.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<GetUserByIdQueryVm>;
