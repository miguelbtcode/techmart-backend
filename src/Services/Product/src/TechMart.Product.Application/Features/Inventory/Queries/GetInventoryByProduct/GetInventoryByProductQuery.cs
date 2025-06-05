using MediatR;
using TechMart.Product.Application.Common.DTOs;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Inventory.Queries.GetInventoryByProduct;

public record GetInventoryByProductQuery(Guid ProductId) : IRequest<Result<InventoryDto>>;