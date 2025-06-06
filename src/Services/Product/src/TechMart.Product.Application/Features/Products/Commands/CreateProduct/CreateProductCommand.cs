using MediatR;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Product.Enums;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    Guid BrandId,
    Guid CategoryId,
    ProductType Type = ProductType.Physical,
    string? ShortDescription = null,
    decimal? CompareAtPrice = null,
    decimal? Weight = null,
    string? WeightUnit = null,
    string? Tags = null
) : IRequest<Result<ProductVm>>;