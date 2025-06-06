using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Product;
using TechMart.Product.Domain.Product.ValueObjects;
using TechMart.SharedKernel.Common;
using ProductEntity = TechMart.Product.Domain.Product.Product;

namespace TechMart.Product.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductVm>>
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProductVm>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Create Sku
            var skuResult = ProductSku.Create(request.Sku);
            if (skuResult.IsFailure)
            {
                return Result.Failure<ProductVm>(skuResult.Error);
            }
            var sku = skuResult.Value;
            
            // Check if SKU already exists
            var skuExists = await _productRepository.SkuExistsAsync(sku, cancellationToken: cancellationToken);
            if (skuExists)
            {
                return Result.Failure<ProductVm>(Error.Conflict("Product.SkuExists", $"A product with SKU '{request.Sku}' already exists"));
            }

            // Validate brand exists
            var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
            if (brand == null)
            {
                return Result.Failure<ProductVm>(Error.NotFound("Brand.NotFound", $"Brand with ID '{request.BrandId}' not found"));
            }

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result.Failure<ProductVm>(Error.NotFound("Category.NotFound", $"Category with ID '{request.CategoryId}' not found"));
            }

            // Create price value object
            var priceResult = Price.Create(request.Price, request.Currency);
            if (priceResult.IsFailure)
            {
                return Result.Failure<ProductVm>(priceResult.Error);
            }
            var price = priceResult.Value;
            
            Price? compareAtPrice = null;
            if (request.CompareAtPrice.HasValue)
            {
                var compareAtPriceResult = Price.Create(request.CompareAtPrice.Value, request.Currency);
                if (compareAtPriceResult.IsFailure)
                {
                    return Result.Failure<ProductVm>(compareAtPriceResult.Error);
                }
                compareAtPrice = compareAtPriceResult.Value;
            }

            // Create weight value object if provided
            Weight? weight = null;
            if (request.Weight.HasValue)
            {
                weight = new Weight(request.Weight.Value, request.WeightUnit ?? "kg");
            }

            // Create product
            var product = new ProductEntity(
                sku,
                request.Name,
                request.Description,
                price,
                request.BrandId,
                request.CategoryId,
                request.Type);

            // Set optional properties
            if (!string.IsNullOrWhiteSpace(request.ShortDescription) || 
                !string.IsNullOrWhiteSpace(request.Tags) || 
                compareAtPrice != null)
            {
                product.UpdateBasicInfo(
                    request.Name,
                    request.Description,
                    request.ShortDescription,
                    tags: request.Tags);
            }

            if (compareAtPrice != null)
            {
                product.UpdatePricing(price, compareAtPrice);
            }

            if (weight != null)
            {
                product.UpdatePhysicalProperties(weight, null);
            }

            // Save product
            await _productRepository.AddAsync(product, cancellationToken);

            _logger.LogInformation("Product created successfully with SKU: {Sku}", request.Sku);

            // Map to DTO and return
            var productDto = _mapper.Map<ProductVm>(product);
            productDto.BrandName = brand.Name;
            productDto.CategoryName = category.Name;

            return Result.Success(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product with SKU: {Sku}", request.Sku);
            return Result.Failure<ProductVm>(Error.Failure("Product.CreateFailed", "Failed to create product"));
        }
    }
}