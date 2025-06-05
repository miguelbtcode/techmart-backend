using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace TechMart.SharedKernel.Extensions.Database;

/// <summary>
/// Extension methods for Entity Framework.
/// </summary>
public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Applies pagination to a queryable.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The paginated queryable.</returns>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Conditionally applies a where clause.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="condition">The condition to check.</param>
    /// <param name="predicate">The predicate to apply if condition is true.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Conditionally applies a where clause with parameter.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="parameter">The parameter value.</param>
    /// <param name="predicate">The predicate to apply if parameter is not null/default.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> WhereIfNotNull<T, TParam>(this IQueryable<T> query, TParam? parameter, Expression<Func<T, bool>> predicate)
        where TParam : class
    {
        return parameter != null ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Conditionally applies a where clause for value types.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="parameter">The parameter value.</param>
    /// <param name="predicate">The predicate to apply if parameter has value.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> WhereIfHasValue<T, TParam>(this IQueryable<T> query, TParam? parameter, Expression<Func<T, bool>> predicate)
        where TParam : struct
    {
        return parameter.HasValue ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Includes related data if the condition is true.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="condition">The condition to check.</param>
    /// <param name="navigationPropertyPath">The navigation property path.</param>
    /// <returns>The queryable with conditional include.</returns>
    public static IQueryable<T> IncludeIf<T, TProperty>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, TProperty>> navigationPropertyPath)
        where T : class
    {
        return condition ? query.Include(navigationPropertyPath) : query;
    }

    /// <summary>
    /// Orders by a property conditionally.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="condition">The condition to check.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="descending">Whether to order descending.</param>
    /// <returns>The ordered queryable.</returns>
    public static IQueryable<T> OrderByIf<T, TKey>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, TKey>> keySelector,
        bool descending = false)
    {
        if (!condition) return query;
        
        return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
    }

    /// <summary>
    /// Applies soft delete filter.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable.</param>
    /// <param name="includeDeleted">Whether to include deleted entities.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> FilterSoftDeleted<T>(this IQueryable<T> query, bool includeDeleted = false)
        where T : class
    {
        if (includeDeleted) return query;

        // Assuming entities have IsDeleted property
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, "IsDeleted");
        var constant = Expression.Constant(false);
        var equal = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

        return query.Where(lambda);
    }
}