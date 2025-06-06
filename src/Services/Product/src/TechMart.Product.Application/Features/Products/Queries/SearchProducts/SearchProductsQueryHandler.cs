using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Features.Products.Vms;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, Result<PaginatedResponseVm<ProductVm>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SearchProductsQueryHandler> _logger;

    public SearchProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<SearchProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedResponseVm<ProductVm>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return Result.Failure<PaginatedResponseVm<ProductVm>>(
                    Error.Validation("Search.EmptyTerm", "Search term cannot be empty"));
            }

            _logger.LogInformation("Searching products with term: {SearchTerm}", request.SearchTerm);

            // Build search specification
            var specification = BuildSearchSpecification(request);

            // Get paginated results
            var pagedProducts = await _productRepository.GetPagedAsync(
                specification,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            // Map to DTOs
            var productDtos = _mapper.Map<List<ProductVm>>(pagedProducts.Items);

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

            _logger.LogInformation("Found {Count} products for search term: {SearchTerm}", 
                pagedProducts.TotalCount, request.SearchTerm);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", request.SearchTerm);
            return Result.Failure<PaginatedResponseVm<ProductVm>>(
                Error.Failure("Products.SearchFailed", "Failed to search products"));
        }
    }

    private Expression<Func<Domain.Product.Product, bool>> BuildSearchSpecification(SearchProductsQuery request)
    {
        var specifications = new List<Expression<Func<Domain.Product.Product, bool>>>();

        // Text search
        specifications.Add(ProductSpecifications.ContainsText(request.SearchTerm));

        // Include only active products unless specified otherwise
        if (!request.IncludeInactive)
        {
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

        // Filter by price range
        if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
        {
            var minPrice = request.MinPrice ?? 0;
            var maxPrice = request.MaxPrice ?? decimal.MaxValue;
            specifications.Add(ProductSpecifications.InPriceRange(minPrice, maxPrice));
        }

        return CombineSpecifications(specifications);
    }

    private Expression<Func<Domain.Product.Product, bool>> CombineSpecifications(
        List<Expression<Func<Domain.Product.Product, bool>>> specifications)
    {
        if (!specifications.Any())
        {
            return x => true;
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