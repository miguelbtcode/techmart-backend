using MediatR;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Queries.GetProductsByBrand;

public record GetProductsByBrandQuery(
    Guid BrandId,
    int PageNumber = 1,
    int PageSize = 10,
    bool IncludeInactive = false,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<Result<PaginatedResponseVm<ProductVm>>>;