using MediatR;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{

}