using AutoMapper;
using MediatR;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetCategoryQueryHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure<CategoryDto>(Error.NotFound("Category.NotFound", 
                $"Category with ID '{request.Id}' not found"));
        }

        var categoryDto = _mapper.Map<CategoryDto>(category);

        // Get product count for this category
        categoryDto.ProductCount = await _productRepository.GetCountByCategoryAsync(request.Id, false, cancellationToken);

        return Result.Success(categoryDto);
    }
}