using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Category;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate parent category if provided
            if (request.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (parentCategory == null)
                {
                    return Result.Failure<CategoryDto>(Error.NotFound("Category.ParentNotFound", 
                        $"Parent category with ID '{request.ParentCategoryId}' not found"));
                }
            }

            // Create category
            var category = new Category(request.Name, request.Description, request.ParentCategoryId);

            // Set optional properties
            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                // Note: In a real implementation, you might want to add SetImageUrl method to Category
                // For now, we'll assume it's set during creation or add it as a method
            }

            // Save category
            await _categoryRepository.AddAsync(category, cancellationToken);

            _logger.LogInformation("Category created successfully: {CategoryId} - {CategoryName}", 
                category.Id, category.Name);

            // Map to DTO and return
            var categoryDto = _mapper.Map<CategoryDto>(category);

            return Result.Success(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category: {CategoryName}", request.Name);
            return Result.Failure<CategoryDto>(Error.Failure("Category.CreateFailed", "Failed to create category"));
        }
    }
}