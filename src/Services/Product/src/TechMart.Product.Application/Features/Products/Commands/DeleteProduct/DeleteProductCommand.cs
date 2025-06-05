using MediatR;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;