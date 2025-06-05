using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Inventory.Commands.UpdateStock;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateStockCommandHandler> _logger;

    public UpdateStockCommandHandler(
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        ILogger<UpdateStockCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify product exists
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                return Result.Failure(Error.NotFound("Product.NotFound", 
                    $"Product with ID '{request.ProductId}' not found"));
            }

            // Get or create inventory record
            var inventory = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
            if (inventory == null)
            {
                inventory = new Domain.Inventory.Inventory(request.ProductId, product.Sku.Value);
                await _inventoryRepository.AddAsync(inventory, cancellationToken);
            }

            _logger.LogInformation("Updating stock for product {ProductId}: {Adjustment} units - {Reason}", 
                request.ProductId, request.QuantityAdjustment, request.Reason);

            // Adjust stock
            inventory.AdjustStock(request.QuantityAdjustment, request.Reason);

            // Update inventory
            _inventoryRepository.Update(inventory);

            _logger.LogInformation("Stock updated successfully for product {ProductId}. New stock: {NewStock}", 
                request.ProductId, inventory.QuantityOnHand);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product: {ProductId}", request.ProductId);
            return Result.Failure(Error.Failure("Inventory.UpdateStockFailed", "Failed to update stock"));
        }
    }
}