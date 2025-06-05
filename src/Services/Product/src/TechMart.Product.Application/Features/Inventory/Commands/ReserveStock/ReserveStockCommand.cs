using MediatR;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Inventory.Commands.ReserveStock;

public record ReserveStockCommand(
    Guid ProductId,
    int Quantity,
    string? ReservationId = null,
    string? ReservedBy = null
) : IRequest<Result>;