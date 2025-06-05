using Dapper;
using System.Data;
using System.Text;

namespace TechMart.SharedKernel.Extensions.Database;

/// <summary>
/// Extension methods for Dapper.
/// </summary>
public static class DapperExtensions
{
    /// <summary>
    /// Executes a query and returns a paged result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="connection">The database connection.</param>
    /// <param name="sql">The SQL query.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="parameters">The query parameters.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <returns>A paged result.</returns>
    public static async Task<(IEnumerable<T> Items, int TotalCount)> QueryPagedAsync<T>(
        this IDbConnection connection,
        string sql,
        int pageNumber,
        int pageSize,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        var offset = (pageNumber - 1) * pageSize;
        
        var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
        var pagedSql = $"{sql} ORDER BY (SELECT NULL) OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

        var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters, transaction, commandTimeout);
        var items = await connection.QueryAsync<T>(pagedSql, parameters, transaction, commandTimeout);

        return (items, totalCount);
    }

    /// <summary>
    /// Executes a query with dynamic WHERE conditions.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="connection">The database connection.</param>
    /// <param name="tableName">The table name.</param>
    /// <param name="whereConditions">The WHERE conditions.</param>
    /// <param name="parameters">The query parameters.</param>
    /// <param name="selectColumns">The columns to select (defaults to *).</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <returns>The query results.</returns>
    public static async Task<IEnumerable<T>> QueryWithConditionsAsync<T>(
        this IDbConnection connection,
        string tableName,
        Dictionary<string, object> whereConditions,
        object? parameters = null,
        string selectColumns = "*",
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        var sql = BuildSelectQuery(tableName, whereConditions, selectColumns);
        var combinedParameters = CombineParameters(parameters, whereConditions);
        
        return await connection.QueryAsync<T>(sql, combinedParameters, transaction, commandTimeout);
    }

    /// <summary>
    /// Executes a bulk insert operation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="connection">The database connection.</param>
    /// <param name="tableName">The table name.</param>
    /// <param name="entities">The entities to insert.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <returns>The number of affected rows.</returns>
    public static async Task<int> BulkInsertAsync<T>(
        this IDbConnection connection,
        string tableName,
        IEnumerable<T> entities,
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        if (!entities.Any()) return 0;

        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.Name != "Id")
            .ToArray();

        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var valueNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));
        
        var sql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({valueNames})";
        
        return await connection.ExecuteAsync(sql, entities, transaction, commandTimeout);
    }

    /// <summary>
    /// Executes a bulk update operation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="connection">The database connection.</param>
    /// <param name="tableName">The table name.</param>
    /// <param name="entities">The entities to update.</param>
    /// <param name="keyColumn">The key column name.</param>
    /// <param name="transaction">The transaction.</param>
    /// <param name="commandTimeout">The command timeout.</param>
    /// <returns>The number of affected rows.</returns>
    public static async Task<int> BulkUpdateAsync<T>(
        this IDbConnection connection,
        string tableName,
        IEnumerable<T> entities,
        string keyColumn = "Id",
        IDbTransaction? transaction = null,
        int? commandTimeout = null)
    {
        if (!entities.Any()) return 0;

        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.Name != keyColumn)
            .ToArray();

        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
        var sql = $"UPDATE {tableName} SET {setClause} WHERE {keyColumn} = @{keyColumn}";
        
        return await connection.ExecuteAsync(sql, entities, transaction, commandTimeout);
    }

    private static string BuildSelectQuery(string tableName, Dictionary<string, object> whereConditions, string selectColumns)
    {
        var sql = new StringBuilder($"SELECT {selectColumns} FROM {tableName}");
        
        if (whereConditions.Any())
        {
            var conditions = whereConditions.Keys.Select(key => $"{key} = @{key}");
            sql.Append($" WHERE {string.Join(" AND ", conditions)}");
        }

        return sql.ToString();
    }

    private static object CombineParameters(object? parameters, Dictionary<string, object> whereConditions)
    {
        var dynamicParameters = new DynamicParameters();
        
        if (parameters != null)
        {
            dynamicParameters.AddDynamicParams(parameters);
        }
        
        foreach (var condition in whereConditions)
        {
            dynamicParameters.Add(condition.Key, condition.Value);
        }

        return dynamicParameters;
    }
}