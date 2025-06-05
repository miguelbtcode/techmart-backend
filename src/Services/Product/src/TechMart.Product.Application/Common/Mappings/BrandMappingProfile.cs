using AutoMapper;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Brand;

namespace TechMart.Product.Application.Common.Mappings;

public class BrandMappingProfile : Profile
{
    public BrandMappingProfile()
    {
        CreateMap<Brand, BrandDto>()
            .ForMember(dest => dest.ProductCount, opt => opt.Ignore()); // Will be calculated separately
    }
}