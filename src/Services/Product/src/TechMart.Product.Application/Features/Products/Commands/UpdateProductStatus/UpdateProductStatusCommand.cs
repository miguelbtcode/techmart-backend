using MediatR;
using TechMart.Product.Domain.Product.Enums;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.UpdateProductStatus;

public record UpdateProductStatusCommand(
    Guid ProductId,
    ProductStatus Status,
    string? UpdatedBy = null
) : IRequest<Result>;