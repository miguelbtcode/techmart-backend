using AutoMapper;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Aggregates.ProductAggregate.Entities;

namespace TechMart.Product.Application.Common.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Domain.Aggregates.ProductAggregate.Entities.Product, ProductDto>()
            .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku.Value))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency))
            .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src => src.CompareAtPrice != null ? src.CompareAtPrice.Amount : (decimal?)null))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight != null ? src.Weight.Value : (decimal?)null))
            .ForMember(dest => dest.WeightUnit, opt => opt.MapFrom(src => src.Weight != null ? src.Weight.Unit : null))
            .ForMember(dest => dest.BrandName, opt => opt.Ignore()) // Will be populated from Brand
            .ForMember(dest => dest.CategoryName, opt => opt.Ignore()) // Will be populated from Category
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AverageRating))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.ReviewCount))
            .ForMember(dest => dest.IsOnSale, opt => opt.MapFrom(src => src.IsOnSale))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage));

        CreateMap<ProductImage, ProductImageDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl.Value));

        CreateMap<ProductVariant, ProductVariantDto>()
            .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku.Value))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src => src.CompareAtPrice != null ? src.CompareAtPrice.Amount : (decimal?)null));

        CreateMap<ProductAttribute, ProductAttributeDto>();
    }
}