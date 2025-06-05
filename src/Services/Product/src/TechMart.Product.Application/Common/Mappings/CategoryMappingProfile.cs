using AutoMapper;
using TechMart.Product.Application.Common.DTOs;
using TechMart.Product.Domain.Aggregates.CategoryAggregate.Entities;

namespace TechMart.Product.Application.Common.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.ProductCount, opt => opt.Ignore()); // Will be calculated separately
    }
}