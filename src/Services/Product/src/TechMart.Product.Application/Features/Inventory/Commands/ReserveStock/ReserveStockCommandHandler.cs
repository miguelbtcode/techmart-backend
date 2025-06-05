using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Inventory.Commands.ReserveStock;

public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, Result>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ReserveStockCommandHandler> _logger;

    public ReserveStockCommandHandler(
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        ILogger<ReserveStockCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Quantity <= 0)
            {
                return Result.Failure(Error.Validation("Inventory.InvalidQuantity", 
                    "Quantity must be greater than zero"));
            }

            // Verify product exists
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                return Result.Failure(Error.NotFound("Product.NotFound", 
                    $"Product with ID '{request.ProductId}' not found"));
            }

            // Get inventory
            var inventory = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
            if (inventory == null)
            {
                return Result.Failure(Error.NotFound("Inventory.NotFound", 
                    $"Inventory for product '{request.ProductId}' not found"));
            }

            _logger.LogInformation("Attempting to reserve {Quantity} units for product {ProductId}", 
                request.Quantity, request.ProductId);

            // Try to reserve stock
            var reservationSuccessful = inventory.TryReserveStock(request.Quantity);
            if (!reservationSuccessful)
            {
                return Result.Failure(Error.Conflict("Inventory.InsufficientStock", 
                    $"Insufficient stock available. Requested: {request.Quantity}, Available: {inventory.AvailableQuantity}"));
            }

            // Update inventory
            _inventoryRepository.Update(inventory);

            _logger.LogInformation("Successfully reserved {Quantity} units for product {ProductId}. Available stock: {AvailableStock}", 
                request.Quantity, request.ProductId, inventory.AvailableQuantity);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving stock for product: {ProductId}", request.ProductId);
            return Result.Failure(Error.Failure("Inventory.ReserveStockFailed", "Failed to reserve stock"));
        }
    }
}