using MediatR;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    Guid BrandId,
    Guid CategoryId,
    string? ShortDescription = null,
    decimal? CompareAtPrice = null,
    decimal? Weight = null,
    string? WeightUnit = null,
    string? Tags = null
) : IRequest<Result<ProductVm>>;