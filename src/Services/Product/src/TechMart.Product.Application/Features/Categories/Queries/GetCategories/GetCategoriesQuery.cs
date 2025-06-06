using MediatR;
using TechMart.Product.Application.Features.Categories.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery(
    Guid? ParentCategoryId = null,
    bool IncludeInactive = false,
    bool IncludeProductCount = true
) : IRequest<Result<List<CategoryVm>>>;