using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Product;
using TechMart.Product.Domain.Product.ValueObjects;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductVm>>
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProductVm>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                return Result.Failure<ProductVm>(Error.NotFound("Product.NotFound", $"Product with ID '{request.Id}' not found"));
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

            // Update basic info
            product.UpdateBasicInfo(
                request.Name,
                request.Description,
                request.ShortDescription,
                tags: request.Tags);

            // Update pricing
            var price = new Price(request.Price, request.Currency);
            var compareAtPrice = request.CompareAtPrice.HasValue 
                ? new Price(request.CompareAtPrice.Value, request.Currency) 
                : null;
            
            product.UpdatePricing(price, compareAtPrice);

            // Update physical properties if needed
            if (request.Weight.HasValue)
            {
                var weight = new Weight(request.Weight.Value, request.WeightUnit ?? "kg");
                product.UpdatePhysicalProperties(weight, null);
            }

            // Update brand and category if changed
            if (product.BrandId != request.BrandId)
            {
                product.ChangeBrand(request.BrandId);
            }

            if (product.CategoryId != request.CategoryId)
            {
                product.ChangeCategory(request.CategoryId);
            }

            // Save changes
            _productRepository.Update(product);

            _logger.LogInformation("Product updated successfully: {ProductId}", request.Id);

            // Map to DTO and return
            var productDto = _mapper.Map<ProductVm>(product);
            productDto.BrandName = brand.Name;
            productDto.CategoryName = category.Name;

            return Result.Success(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product: {ProductId}", request.Id);
            return Result.Failure<ProductVm>(Error.Failure("Product.UpdateFailed", "Failed to update product"));
        }
    }
}