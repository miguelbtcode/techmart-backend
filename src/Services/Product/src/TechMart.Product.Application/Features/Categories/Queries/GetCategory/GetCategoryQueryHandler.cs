using AutoMapper;
using MediatR;
using TechMart.Product.Application.Features.Categories.Vms;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler(
    ICategoryRepository categoryRepository,
    IProductRepository productRepository,
    IMapper mapper)
    : IRequestHandler<GetCategoryQuery, Result<CategoryVm>>
{
    public async Task<Result<CategoryVm>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure<CategoryVm>(Error.NotFound("Category.NotFound", 
                $"Category with ID '{request.Id}' not found"));
        }

        var categoryDto = mapper.Map<CategoryVm>(category);

        // Get product count for this category
        categoryDto.ProductCount = await productRepository.GetCountByCategoryAsync(request.Id, false, cancellationToken);

        return Result.Success(categoryDto);
    }
}