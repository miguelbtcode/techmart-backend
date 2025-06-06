using MediatR;
using TechMart.Product.Application.Features.Brands.Vms;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Features.Brands.Commands.CreateBrand;

public record CreateBrandCommand(
    string Name,
    string? Description = null,
    string? LogoUrl = null,
    string? WebsiteUrl = null
) : IRequest<Result<BrandVm>>;