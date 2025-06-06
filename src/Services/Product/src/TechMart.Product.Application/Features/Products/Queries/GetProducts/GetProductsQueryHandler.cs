using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;
using ProductEntity = TechMart.Product.Domain.Product.Product;

namespace TechMart.Product.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PaginatedResponseVm<ProductVm>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedResponseVm<ProductVm>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting products with filters: {@Filters}", request);

            // Build the specification based on query parameters
            var specification = BuildSpecification(request);

            // Get paginated products
            var pagedProducts = await _productRepository.GetPagedAsync(
                specification,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            // Map to DTOs
            var productDtos = _mapper.Map<List<ProductVm>>(pagedProducts.Items);

            // Create paginated response
            var response = new PaginatedResponseVm<ProductVm>
            {
                Items = productDtos,
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize,
                TotalCount = pagedProducts.TotalCount,
                TotalPages = pagedProducts.TotalPages,
                HasPreviousPage = pagedProducts.HasPreviousPage,
                HasNextPage = pagedProducts.HasNextPage
            };

            _logger.LogInformation("Successfully retrieved {Count} products", productDtos.Count);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products with filters: {@Filters}", request);
            return Result.Failure<PaginatedResponseVm<ProductVm>>(
                Error.Failure("Products.GetFailed", "Failed to retrieve products"));
        }
    }

    private Expression<Func<ProductEntity, bool>> BuildSpecification(GetProductsQuery request)
    {
        var specifications = new List<Expression<Func<ProductEntity, bool>>>();

        // Filter by status
        if (request.Status.HasValue)
        {
            specifications.Add(ProductSpecifications.HasStatus(request.Status.Value));
        }
        else
        {
            // Default to active products if no status specified
            specifications.Add(ProductSpecifications.IsActive());
        }

        // Filter by category
        if (request.CategoryId.HasValue)
        {
            specifications.Add(ProductSpecifications.InCategory(request.CategoryId.Value));
        }

        // Filter by brand
        if (request.BrandId.HasValue)
        {
            specifications.Add(ProductSpecifications.OfBrand(request.BrandId.Value));
        }

        // Filter by featured
        if (request.IsFeatured.HasValue && request.IsFeatured.Value)
        {
            specifications.Add(ProductSpecifications.IsFeatured());
        }

        // Filter by on sale
        if (request.IsOnSale.HasValue && request.IsOnSale.Value)
        {
            specifications.Add(ProductSpecifications.IsOnSale());
        }

        // Filter by price range
        if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
        {
            var minPrice = request.MinPrice ?? 0;
            var maxPrice = request.MaxPrice ?? decimal.MaxValue;
            specifications.Add(ProductSpecifications.InPriceRange(minPrice, maxPrice));
        }

        // Combine all specifications
        return CombineSpecifications(specifications);
    }

    private Expression<Func<ProductEntity, bool>> CombineSpecifications(
        List<Expression<Func<ProductEntity, bool>>> specifications)
    {
        if (!specifications.Any())
        {
            return x => true; // Return all if no specifications
        }

        var combinedSpec = specifications.First();

        foreach (var spec in specifications.Skip(1))
        {
            combinedSpec = CombineWithAnd(combinedSpec, spec);
        }

        return combinedSpec;
    }

    private Expression<Func<T, bool>> CombineWithAnd<T>(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T));
        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    private Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}