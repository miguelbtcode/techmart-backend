using AutoMapper;
using TechMart.Product.Application.Features.Categories.Vms;
using TechMart.Product.Domain.Category;

namespace TechMart.Product.Application.Common.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryVm>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.ProductCount, opt => opt.Ignore()); // Will be calculated separately
    }
}