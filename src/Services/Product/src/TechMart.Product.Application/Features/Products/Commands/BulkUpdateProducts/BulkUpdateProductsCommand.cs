using MediatR;
using TechMart.Product.Domain.Product.Enums;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.BulkUpdateProducts;

public record BulkUpdateProductsCommand(
    IEnumerable<Guid> ProductIds,
    ProductStatus? Status = null,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    bool? IsFeatured = null,
    string? UpdatedBy = null
) : IRequest<Result>;