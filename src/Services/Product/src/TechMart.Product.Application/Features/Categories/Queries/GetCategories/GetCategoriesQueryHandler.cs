using AutoMapper;
using MediatR;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories;

        if (request.ParentCategoryId.HasValue)
        {
            categories = await _categoryRepository.GetByParentAsync(request.ParentCategoryId, cancellationToken);
        }
        else if (!request.IncludeInactive)
        {
            categories = await _categoryRepository.GetActiveAsync(cancellationToken);
        }
        else
        {
            categories = await _categoryRepository.GetAllAsync(cancellationToken);
        }

        var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

        // Include product count if requested
        if (request.IncludeProductCount)
        {
            foreach (var categoryDto in categoryDtos)
            {
                categoryDto.ProductCount = await _productRepository.GetCountByCategoryAsync(
                    categoryDto.Id, false, cancellationToken);
            }
        }

        return Result.Success(categoryDtos);
    }
}