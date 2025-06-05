using MediatR;
using TechMart.Product.Application.Common.DTOs;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description = null,
    Guid? ParentCategoryId = null,
    string? ImageUrl = null,
    int SortOrder = 0
) : IRequest<Result<CategoryDto>>;