using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.BulkUpdateProducts;

public class BulkUpdateProductsCommandHandler : IRequestHandler<BulkUpdateProductsCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<BulkUpdateProductsCommandHandler> _logger;

    public BulkUpdateProductsCommandHandler(
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository,
        ILogger<BulkUpdateProductsCommandHandler> logger)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(BulkUpdateProductsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var productIds = request.ProductIds.ToList();
            if (!productIds.Any())
            {
                return Result.Failure(Error.Validation("BulkUpdate.EmptyList", "Product IDs list cannot be empty"));
            }

            _logger.LogInformation("Starting bulk update for {Count} products", productIds.Count);

            // Validate category if provided
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
                if (category == null)
                {
                    return Result.Failure(Error.NotFound("Category.NotFound", $"Category with ID '{request.CategoryId}' not found"));
                }
            }

            // Validate brand if provided
            if (request.BrandId.HasValue)
            {
                var brand = await _brandRepository.GetByIdAsync(request.BrandId.Value, cancellationToken);
                if (brand == null)
                {
                    return Result.Failure(Error.NotFound("Brand.NotFound", $"Brand with ID '{request.BrandId}' not found"));
                }
            }

            var updatedCount = 0;

            // Use bulk operations if available in repository
            if (request.Status.HasValue)
            {
                updatedCount += await _productRepository.BulkUpdateStatusAsync(productIds, request.Status.Value, request.UpdatedBy, cancellationToken);
            }

            if (request.CategoryId.HasValue)
            {
                updatedCount += await _productRepository.BulkUpdateCategoryAsync(productIds, request.CategoryId.Value, request.UpdatedBy, cancellationToken);
            }

            if (request.IsFeatured.HasValue)
            {
                updatedCount += await _productRepository.BulkSetFeaturedAsync(productIds, request.IsFeatured.Value, request.UpdatedBy, cancellationToken);
            }

            // For brand updates and other complex operations, load and update individually
            if (request.BrandId.HasValue)
            {
                var products = await _productRepository.GetBySpecificationAsync(p => productIds.Contains(p.Id), cancellationToken);
                foreach (var product in products)
                {
                    product.ChangeBrand(request.BrandId.Value, request.UpdatedBy);
                    _productRepository.Update(product);
                    updatedCount++;
                }
            }

            _logger.LogInformation("Bulk update completed. {UpdatedCount} products updated", updatedCount);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk update of products");
            return Result.Failure(Error.Failure("Product.BulkUpdateFailed", "Failed to bulk update products"));
        }
    }
}