using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Queries.GetProductsByBrand;

public class GetProductsByBrandQueryHandler : IRequestHandler<GetProductsByBrandQuery, Result<PaginatedResponseVm<ProductVm>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsByBrandQueryHandler> _logger;

    public GetProductsByBrandQueryHandler(
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        IMapper mapper,
        ILogger<GetProductsByBrandQueryHandler> logger)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedResponseVm<ProductVm>>> Handle(GetProductsByBrandQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate brand exists
            var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
            if (brand == null)
            {
                return Result.Failure<PaginatedResponseVm<ProductVm>>(
                    Error.NotFound("Brand.NotFound", $"Brand with ID '{request.BrandId}' not found"));
            }

            _logger.LogInformation("Getting products for brand: {BrandId} - {BrandName}", 
                request.BrandId, brand.Name);

            // Get paginated products by brand
            var pagedProducts = await _productRepository.GetPagedByBrandAsync(
                request.BrandId,
                request.PageNumber,
                request.PageSize,
                request.IncludeInactive,
                cancellationToken);

            // Map to DTOs
            var productDtos = _mapper.Map<List<ProductVm>>(pagedProducts.Items);

            // Set brand name for all products
            foreach (var productDto in productDtos)
            {
                productDto.BrandName = brand.Name;
            }

            var response = new PaginatedResponseVm<ProductVm>
            {
                Items = productDtos,
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize,
                TotalCount = pagedProducts.TotalCount,
                TotalPages = pagedProducts.TotalPages,
                HasPreviousPage = pagedProducts.HasPreviousPage,
                HasNextPage = pagedProducts.HasNextPage
            };

            _logger.LogInformation("Found {Count} products for brand: {BrandName}", 
                pagedProducts.TotalCount, brand.Name);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for brand: {BrandId}", request.BrandId);
            return Result.Failure<PaginatedResponseVm<ProductVm>>(
                Error.Failure("Products.GetByBrandFailed", "Failed to retrieve products by brand"));
        }
    }
}