using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Features.Brands.Vms;
using TechMart.Product.Domain.Brand;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommandHandler(
    IBrandRepository brandRepository,
    IMapper mapper,
    ILogger<CreateBrandCommandHandler> logger)
    : IRequestHandler<CreateBrandCommand, Result<BrandVm>>
{
    public async Task<Result<BrandVm>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if brand name already exists
            var nameExists = await brandRepository.NameExistsAsync(request.Name, null, cancellationToken);
            if (nameExists)
            {
                return Result.Failure<BrandVm>(Error.Conflict("Brand.NameExists", 
                    $"A brand with name '{request.Name}' already exists"));
            }

            // Create brand
            var brand = new Brand(request.Name, request.Description);

            // Set optional properties
            if (!string.IsNullOrWhiteSpace(request.LogoUrl))
            {
                brand.SetLogo(request.LogoUrl);
            }

            if (!string.IsNullOrWhiteSpace(request.WebsiteUrl))
            {
                brand.SetWebsite(request.WebsiteUrl);
            }

            // Save brand
            await brandRepository.AddAsync(brand, cancellationToken);

            logger.LogInformation("Brand created successfully: {BrandId} - {BrandName}", 
                brand.Id, brand.Name);

            // Map to DTO and return
            var brandDto = mapper.Map<BrandVm>(brand);

            return Result.Success(brandDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating brand: {BrandName}", request.Name);
            return Result.Failure<BrandVm>(Error.Failure("Brand.CreateFailed", "Failed to create brand"));
        }
    }
}