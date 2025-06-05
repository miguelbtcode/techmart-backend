using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Domain.Product;
using TechMart.Product.Domain.Product.Events;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                return Result.Failure(Error.NotFound("Product.NotFound", $"Product with ID '{request.Id}' not found"));
            }

            _logger.LogInformation("Deleting product: {ProductId} - {ProductName}", request.Id, product.Name);

            // Add domain event before deleting
            product.AddDomainEvent(new ProductDeletedEvent(product.Id, product.Sku.Value, product.Name));

            // Remove the product
            _productRepository.Remove(product);

            _logger.LogInformation("Product deleted successfully: {ProductId}", request.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product: {ProductId}", request.Id);
            return Result.Failure(Error.Failure("Product.DeleteFailed", "Failed to delete product"));
        }
    }
}