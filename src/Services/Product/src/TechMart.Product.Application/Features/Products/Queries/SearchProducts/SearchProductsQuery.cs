using MediatR;
using TechMart.Product.Application.Common.DTOs;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool IncludeInactive = false
) : IRequest<Result<PaginatedResponseDto<ProductDto>>>;