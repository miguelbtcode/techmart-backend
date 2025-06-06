using AutoMapper;
using MediatR;
using TechMart.Product.Application.Features.Inventory.Vms;
using TechMart.Product.Domain.Inventory;
using TechMart.Product.Domain.Product;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Inventory.Queries.GetInventoryByProduct;

public class GetInventoryByProductQueryHandler : IRequestHandler<GetInventoryByProductQuery, Result<InventoryVm>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetInventoryByProductQueryHandler(
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<InventoryVm>> Handle(GetInventoryByProductQuery request, CancellationToken cancellationToken)
    {
        // Verify product exists
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Failure<InventoryVm>(Error.NotFound("Product.NotFound", 
                $"Product with ID '{request.ProductId}' not found"));
        }

        // Get inventory
        var inventory = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (inventory == null)
        {
            return Result.Failure<InventoryVm>(Error.NotFound("Inventory.NotFound", 
                $"Inventory for product '{request.ProductId}' not found"));
        }

        var inventoryDto = _mapper.Map<InventoryVm>(inventory);
        
        return Result.Success(inventoryDto);
    }
}