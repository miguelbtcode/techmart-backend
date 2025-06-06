using AutoMapper;
using MediatR;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Queries.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductVm>>
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetProductQueryHandler(
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<ProductVm>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return Result.Failure<ProductVm>(Error.NotFound("Product.NotFound", $"Product with ID '{request.Id}' not found"));
        }

        var productDto = _mapper.Map<ProductVm>(product);

        // Load brand and category names
        var brand = await _brandRepository.GetByIdAsync(product.BrandId, cancellationToken);
        var category = await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken);

        productDto.BrandName = brand?.Name;
        productDto.CategoryName = category?.Name;

        return Result.Success(productDto);
    }
}