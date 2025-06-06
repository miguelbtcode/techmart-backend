using MediatR;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Product.Enums;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    ProductStatus? Status = null,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    bool? IsFeatured = null,
    bool? IsOnSale = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<Result<PaginatedResponseVm<ProductVm>>>;