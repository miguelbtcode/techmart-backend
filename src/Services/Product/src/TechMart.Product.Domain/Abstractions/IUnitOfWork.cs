using TechMart.Product.Domain.Brand;
using TechMart.Product.Domain.Category;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Domain.Product;
using IUnitOfWorkBase = TechMart.SharedKernel.Abstractions.IUnitOfWork;

namespace TechMart.Product.Domain.Abstractions;

/// <summary>
/// Extended Unit of Work interface that provides access to all domain repositories.
/// </summary>
public interface IUnitOfWork : IUnitOfWorkBase
{
    /// <summary>
    /// Gets the Product repository.
    /// </summary>
    IProductRepository Products { get; }

    /// <summary>
    /// Gets the Brand repository.
    /// </summary>
    IBrandRepository Brands { get; }

    /// <summary>
    /// Gets the Category repository.
    /// </summary>
    ICategoryRepository Categories { get; }

    /// <summary>
    /// Gets the Inventory repository.
    /// </summary>
    IInventoryRepository Inventories { get; }

    /// <summary>
    /// Executes a function within a transaction scope.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="operation">The operation to execute within the transaction.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a transaction scope.
    /// </summary>
    /// <param name="operation">The operation to execute within the transaction.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value indicating whether a transaction is currently in progress.
    /// </summary>
    bool IsInTransaction { get; }
}