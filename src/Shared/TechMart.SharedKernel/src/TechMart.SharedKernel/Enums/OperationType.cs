using TechMart.SharedKernel.Base;

namespace TechMart.SharedKernel.Enums;

/// <summary>
/// Represents the type of operation being performed.
/// </summary>
public enum OperationType
{
    /// <summary>
    /// Create operation - adding new data.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Read operation - retrieving data.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Update operation - modifying existing data.
    /// </summary>
    Update = 3,

    /// <summary>
    /// Delete operation - removing data.
    /// </summary>
    Delete = 4,

    /// <summary>
    /// Search operation - querying data.
    /// </summary>
    Search = 5,

    /// <summary>
    /// Export operation - extracting data.
    /// </summary>
    Export = 6,

    /// <summary>
    /// Import operation - loading data.
    /// </summary>
    Import = 7,

    /// <summary>
    /// Synchronization operation - syncing data.
    /// </summary>
    Sync = 8,

    /// <summary>
    /// Backup operation - creating backups.
    /// </summary>
    Backup = 9,

    /// <summary>
    /// Restore operation - restoring from backups.
    /// </summary>
    Restore = 10
}

/// <summary>
/// Type-safe enumeration for OperationType with additional functionality.
/// </summary>
public class OperationTypeEnumeration : Enumeration
{
    public static readonly OperationTypeEnumeration Create = new(1, nameof(Create), "CREATE");
    public static readonly OperationTypeEnumeration Read = new(2, nameof(Read), "READ");
    public static readonly OperationTypeEnumeration Update = new(3, nameof(Update), "UPDATE");
    public static readonly OperationTypeEnumeration Delete = new(4, nameof(Delete), "DELETE");
    public static readonly OperationTypeEnumeration Search = new(5, nameof(Search), "SEARCH");
    public static readonly OperationTypeEnumeration Export = new(6, nameof(Export), "EXPORT");
    public static readonly OperationTypeEnumeration Import = new(7, nameof(Import), "IMPORT");
    public static readonly OperationTypeEnumeration Sync = new(8, nameof(Sync), "SYNC");
    public static readonly OperationTypeEnumeration Backup = new(9, nameof(Backup), "BACKUP");
    public static readonly OperationTypeEnumeration Restore = new(10, nameof(Restore), "RESTORE");

    /// <summary>
    /// Gets the HTTP verb typically associated with this operation.
    /// </summary>
    public string HttpVerb { get; }

    private OperationTypeEnumeration(int id, string name, string httpVerb) : base(id, name)
    {
        HttpVerb = httpVerb;
    }

    /// <summary>
    /// Gets a value indicating whether the operation modifies data.
    /// </summary>
    public bool IsModifying => this == Create || this == Update || this == Delete || this == Import || this == Restore;

    /// <summary>
    /// Gets a value indicating whether the operation is read-only.
    /// </summary>
    public bool IsReadOnly => this == Read || this == Search || this == Export;

    /// <summary>
    /// Gets a value indicating whether the operation requires special permissions.
    /// </summary>
    public bool RequiresElevatedPermissions => this == Delete || this == Import || this == Backup || this == Restore;

    /// <summary>
    /// Gets a value indicating whether the operation should be audited.
    /// </summary>
    public bool ShouldAudit => IsModifying || this == Export || this == Backup;

    /// <summary>
    /// Gets CRUD operations only.
    /// </summary>
    public static IEnumerable<OperationTypeEnumeration> GetCrudOperations() =>
        new[] { Create, Read, Update, Delete };

    /// <summary>
    /// Gets data transfer operations.
    /// </summary>
    public static IEnumerable<OperationTypeEnumeration> GetDataTransferOperations() =>
        new[] { Export, Import, Sync, Backup, Restore };

    /// <summary>
    /// Gets the appropriate HTTP status code for successful operations.
    /// </summary>
    public int GetSuccessStatusCode() => this switch
    {
        _ when this == Create => 201, // Created
        _ when this == Read => 200,   // OK
        _ when this == Update => 200, // OK
        _ when this == Delete => 204, // No Content
        _ when this == Search => 200, // OK
        _ => 200 // Default OK
    };

    /// <summary>
    /// Converts a string to an OperationType enumeration.
    /// </summary>
    /// <param name="operationType">The operation type string.</param>
    /// <returns>The corresponding OperationType enumeration.</returns>
    public static OperationTypeEnumeration FromString(string operationType) =>
        operationType.ToUpperInvariant() switch
        {
            "CREATE" or "POST" => Create,
            "READ" or "GET" => Read,
            "UPDATE" or "PUT" or "PATCH" => Update,
            "DELETE" => Delete,
            "SEARCH" => Search,
            "EXPORT" => Export,
            "IMPORT" => Import,
            "SYNC" => Sync,
            "BACKUP" => Backup,
            "RESTORE" => Restore,
            _ => throw new ArgumentException($"Unknown operation type: {operationType}", nameof(operationType))
        };
}