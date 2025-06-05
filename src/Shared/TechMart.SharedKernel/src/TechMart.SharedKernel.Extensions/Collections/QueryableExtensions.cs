using System.Linq.Expressions;
using TechMart.SharedKernel.Common;

namespace TechMart.SharedKernel.Extensions.Collections;

/// <summary>
/// Extension methods for IQueryable<T>.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Conditionally applies a where clause.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Conditionally applies a where clause with parameter validation.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="parameter">The parameter value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> WhereIfNotNull<T, TParam>(this IQueryable<T> query, TParam? parameter, Expression<Func<T, bool>> predicate)
        where TParam : class
    {
        return parameter != null ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Conditionally applies a where clause for nullable value types.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="parameter">The parameter value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> WhereIfHasValue<T, TParam>(this IQueryable<T> query, TParam? parameter, Expression<Func<T, bool>> predicate)
        where TParam : struct
    {
        return parameter.HasValue ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Conditionally applies a where clause for strings.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="parameter">The string parameter.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> WhereIfNotEmpty<T>(this IQueryable<T> query, string? parameter, Expression<Func<T, bool>> predicate)
    {
        return !string.IsNullOrWhiteSpace(parameter) ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Applies pagination to the queryable.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The paginated queryable.</returns>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Orders by a property name dynamically.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="descending">Whether to order descending.</param>
    /// <returns>The ordered queryable.</returns>
    public static IQueryable<T> OrderByProperty<T>(this IQueryable<T> query, string propertyName, bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        
        try
        {
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(resultExpression);
        }
        catch (ArgumentException)
        {
            // Property doesn't exist, return original query
            return query;
        }
    }

    /// <summary>
    /// Adds additional ordering by a property name.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable (must already be ordered).</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="descending">Whether to order descending.</param>
    /// <returns>The ordered queryable.</returns>
    public static IQueryable<T> ThenByProperty<T>(this IOrderedQueryable<T> query, string propertyName, bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        
        try
        {
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = descending ? "ThenByDescending" : "ThenBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda));

            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExpression);
        }
        catch (ArgumentException)
        {
            // Property doesn't exist, return original query
            return query;
        }
    }

    /// <summary>
    /// Creates a PagedList from the queryable.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A paged list.</returns>
    public static PagedList<T> ToPagedList<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var totalCount = query.Count();
        var items = query.Paginate(pageNumber, pageSize).ToList();
        
        return new PagedList<T>(items, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// Creates a PagedList from the queryable asynchronously.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged list.</returns>
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        return await PagedList<T>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
    }

    /// <summary>
    /// Applies search filter to string properties.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="propertyNames">The property names to search.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> Search<T>(this IQueryable<T> query, string? searchTerm, params string[] propertyNames)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || !propertyNames.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var searchExpression = BuildSearchExpression<T>(parameter, searchTerm, propertyNames);
        
        if (searchExpression == null)
            return query;

        var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Applies a filter for soft deleted entities.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="includeDeleted">Whether to include deleted entities.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> FilterSoftDeleted<T>(this IQueryable<T> query, bool includeDeleted = false)
    {
        if (includeDeleted) 
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        
        try
        {
            var property = Expression.Property(parameter, "IsDeleted");
            var constant = Expression.Constant(false);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return query.Where(lambda);
        }
        catch (ArgumentException)
        {
            // IsDeleted property doesn't exist, return original query
            return query;
        }
    }

    /// <summary>
    /// Applies a filter for active entities.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="activeOnly">Whether to include only active entities.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> FilterActive<T>(this IQueryable<T> query, bool activeOnly = true)
    {
        if (!activeOnly) 
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        
        try
        {
            var property = Expression.Property(parameter, "IsActive");
            var constant = Expression.Constant(true);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return query.Where(lambda);
        }
        catch (ArgumentException)
        {
            // IsActive property doesn't exist, return original query
            return query;
        }
    }

    private static Expression? BuildSearchExpression<T>(ParameterExpression parameter, string searchTerm, string[] propertyNames)
    {
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        
        if (containsMethod == null || toLowerMethod == null)
            return null;

        var searchTermLower = searchTerm.ToLower();
        Expression? searchExpression = null;

        foreach (var propertyName in propertyNames)
        {
            try
            {
                var property = Expression.Property(parameter, propertyName);
                
                // Handle different property types
                Expression stringProperty;
                if (property.Type == typeof(string))
                {
                    stringProperty = property;
                }
                else
                {
                    // Convert to string
                    var toStringMethod = property.Type.GetMethod("ToString", Type.EmptyTypes);
                    if (toStringMethod == null) continue;
                    stringProperty = Expression.Call(property, toStringMethod);
                }

                // Add null check for string properties
                var nullCheck = Expression.NotEqual(stringProperty, Expression.Constant(null, typeof(string)));
                var toLowerCall = Expression.Call(stringProperty, toLowerMethod);
                var containsCall = Expression.Call(toLowerCall, containsMethod, Expression.Constant(searchTermLower));
                var propertyCondition = Expression.AndAlso(nullCheck, containsCall);

                searchExpression = searchExpression == null 
                    ? propertyCondition 
                    : Expression.OrElse(searchExpression, propertyCondition);
            }
            catch (ArgumentException)
            {
                // Property doesn't exist, skip it
                continue;
            }
        }

        return searchExpression;
    }
}