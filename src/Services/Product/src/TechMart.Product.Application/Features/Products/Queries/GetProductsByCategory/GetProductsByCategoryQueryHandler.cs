using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Aggregates.CategoryAggregate.Repositories;
using TechMart.Product.Domain.Aggregates.ProductAggregate.Repositories;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Queries.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, Result<PaginatedResponseDto<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsByCategoryQueryHandler> _logger;

    public GetProductsByCategoryQueryHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<GetProductsByCategoryQueryHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedResponseDto<ProductDto>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result.Failure<PaginatedResponseDto<ProductDto>>(
                    Error.NotFound("Category.NotFound", $"Category with ID '{request.CategoryId}' not found"));
            }

            _logger.LogInformation("Getting products for category: {CategoryId} - {CategoryName}", 
                request.CategoryId, category.Name);

            // Get paginated products by category
            var pagedProducts = await _productRepository.GetPagedByCategoryAsync(
                request.CategoryId,
                request.PageNumber,
                request.PageSize,
                request.IncludeInactive,
                cancellationToken);

            // Map to DTOs
            var productDtos = _mapper.Map<List<ProductDto>>(pagedProducts.Items);

            // Set category name for all products
            foreach (var productDto in productDtos)
            {
                productDto.CategoryName = category.Name;
            }

            var response = new PaginatedResponseDto<ProductDto>
            {
                Items = productDtos,
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize,
                TotalCount = pagedProducts.TotalCount,
                TotalPages = pagedProducts.TotalPages,
                HasPreviousPage = pagedProducts.HasPreviousPage,
                HasNextPage = pagedProducts.HasNextPage
            };

            _logger.LogInformation("Found {Count} products in category: {CategoryName}", 
                pagedProducts.TotalCount, category.Name);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for category: {CategoryId}", request.CategoryId);
            return Result.Failure<PaginatedResponseDto<ProductDto>>(
                Error.Failure("Products.GetByCategoryFailed", "Failed to retrieve products by category"));
        }
    }
}