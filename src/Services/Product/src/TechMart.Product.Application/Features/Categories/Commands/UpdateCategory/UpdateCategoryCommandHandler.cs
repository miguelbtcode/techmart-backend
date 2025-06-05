using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Category;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<UpdateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing category
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category == null)
            {
                return Result.Failure<CategoryDto>(Error.NotFound("Category.NotFound", 
                    $"Category with ID '{request.Id}' not found"));
            }

            // Validate parent category if provided and different from current
            if (request.ParentCategoryId.HasValue && request.ParentCategoryId != category.ParentCategoryId)
            {
                // Check if parent exists
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (parentCategory == null)
                {
                    return Result.Failure<CategoryDto>(Error.NotFound("Category.ParentNotFound", 
                        $"Parent category with ID '{request.ParentCategoryId}' not found"));
                }

                // Prevent circular reference - category cannot be its own parent or grandparent
                if (request.ParentCategoryId == request.Id)
                {
                    return Result.Failure<CategoryDto>(Error.Validation("Category.CircularReference", 
                        "Category cannot be its own parent"));
                }

                // Check if the new parent is not a child of this category
                var hierarchy = await _categoryRepository.GetHierarchyAsync(request.ParentCategoryId.Value, cancellationToken);
                if (hierarchy.Any(c => c.Id == request.Id))
                {
                    return Result.Failure<CategoryDto>(Error.Validation("Category.CircularReference", 
                        "Category cannot be moved to one of its child categories"));
                }
            }

            _logger.LogInformation("Updating category: {CategoryId} - {CategoryName}", request.Id, request.Name);

            // Update category
            category.UpdateInfo(request.Name, request.Description);
            
            if (request.ParentCategoryId != category.ParentCategoryId)
            {
                category.SetParent(request.ParentCategoryId);
            }

            // Save changes
            _categoryRepository.Update(category);

            _logger.LogInformation("Category updated successfully: {CategoryId}", request.Id);

            // Map to DTO and return
            var categoryDto = _mapper.Map<CategoryDto>(category);

            return Result.Success(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category: {CategoryId}", request.Id);
            return Result.Failure<CategoryDto>(Error.Failure("Category.UpdateFailed", "Failed to update category"));
        }
    }
}