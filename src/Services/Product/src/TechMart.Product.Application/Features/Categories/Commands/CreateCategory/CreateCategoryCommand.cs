using MediatR;
using TechMart.Product.Application.Features.Categories.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description = null,
    Guid? ParentCategoryId = null,
    string? ImageUrl = null,
    int SortOrder = 0
) : IRequest<Result<CategoryVm>>;