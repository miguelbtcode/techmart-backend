using AutoMapper;
using TechMart.Product.Application.Features.Brands.Vms;
using TechMart.Product.Domain.Brand;

namespace TechMart.Product.Application.Common.Mappings;

public class BrandMappingProfile : Profile
{
    public BrandMappingProfile()
    {
        CreateMap<Brand, BrandVm>()
            .ForMember(dest => dest.ProductCount, opt => opt.Ignore()); // Will be calculated separately
    }
}