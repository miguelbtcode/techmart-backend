using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Aggregates.BrandAggregate.Repositories;
using TechMart.Product.Domain.Aggregates.CategoryAggregate.Repositories;
using TechMart.Product.Domain.Aggregates.ProductAggregate.Repositories;
using TechMart.Product.Domain.Aggregates.ProductAggregate.ValueObjects;
using TechMart.SharedKernel.Common;
using ProductEntity = TechMart.Product.Domain.Aggregates.ProductAggregate.Entities.Product;

namespace TechMart.Product.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
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

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if SKU already exists
            var skuExists = await _productRepository.SkuExistsAsync(request.Sku, cancellationToken: cancellationToken);
            if (skuExists)
            {
                return Result.Failure<ProductDto>(Error.Conflict("Product.SkuExists", $"A product with SKU '{request.Sku}' already exists"));
            }

            // Validate brand exists
            var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);
            if (brand == null)
            {
                return Result.Failure<ProductDto>(Error.NotFound("Brand.NotFound", $"Brand with ID '{request.BrandId}' not found"));
            }

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result.Failure<ProductDto>(Error.NotFound("Category.NotFound", $"Category with ID '{request.CategoryId}' not found"));
            }

            // Create price value object
            var price = new Price(request.Price, request.Currency);
            var compareAtPrice = request.CompareAtPrice.HasValue 
                ? new Price(request.CompareAtPrice.Value, request.Currency) 
                : null;

            // Create weight value object if provided
            Weight? weight = null;
            if (request.Weight.HasValue)
            {
                weight = new Weight(request.Weight.Value, request.WeightUnit ?? "kg");
            }

            // Create product
            var product = new ProductEntity(
                request.Sku,
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
            var productDto = _mapper.Map<ProductDto>(product);
            productDto.BrandName = brand.Name;
            productDto.CategoryName = category.Name;

            return Result.Success(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product with SKU: {Sku}", request.Sku);
            return Result.Failure<ProductDto>(Error.Failure("Product.CreateFailed", "Failed to create product"));
        }
    }
}