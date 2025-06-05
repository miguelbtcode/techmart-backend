using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Products.Commands.UpdateProductStatus;

public class UpdateProductStatusCommandHandler : IRequestHandler<UpdateProductStatusCommand, Result>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductStatusCommandHandler> _logger;

    public UpdateProductStatusCommandHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductStatusCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                return Result.Failure(Error.NotFound("Product.NotFound", $"Product with ID '{request.ProductId}' not found"));
            }

            _logger.LogInformation("Updating product status: {ProductId} from {OldStatus} to {NewStatus}",
                request.ProductId, product.Status, request.Status);

            // Update status
            product.ChangeStatus(request.Status, request.UpdatedBy);

            // Save changes
            _productRepository.Update(product);

            _logger.LogInformation("Product status updated successfully: {ProductId}", request.ProductId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product status: {ProductId}", request.ProductId);
            return Result.Failure(Error.Failure("Product.UpdateStatusFailed", "Failed to update product status"));
        }
    }
}