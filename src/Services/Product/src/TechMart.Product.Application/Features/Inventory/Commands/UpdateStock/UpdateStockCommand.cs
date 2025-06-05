using MediatR;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Inventory.Commands.UpdateStock;

public record UpdateStockCommand(
    Guid ProductId,
    int QuantityAdjustment,
    string Reason,
    string? UpdatedBy = null
) : IRequest<Result>;