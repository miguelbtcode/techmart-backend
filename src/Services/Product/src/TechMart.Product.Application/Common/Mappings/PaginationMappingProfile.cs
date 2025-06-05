using AutoMapper;
using TechMart.Product.Application.Common.DTOs;
using TechMart.SharedKernel.Common;

namespace TechMart.Product.Application.Common.Mappings;

public class PaginationMappingProfile : Profile
{
    public PaginationMappingProfile()
    {
        CreateMap(typeof(PagedList<>), typeof(PaginatedResponseDto<>))
            .ForMember("Items", opt => opt.MapFrom("Items"))
            .ForMember("PageNumber", opt => opt.MapFrom("PageNumber"))
            .ForMember("PageSize", opt => opt.MapFrom("PageSize"))
            .ForMember("TotalCount", opt => opt.MapFrom("TotalCount"))
            .ForMember("TotalPages", opt => opt.MapFrom("TotalPages"))
            .ForMember("HasPreviousPage", opt => opt.MapFrom("HasPreviousPage"))
            .ForMember("HasNextPage", opt => opt.MapFrom("HasNextPage"));
    }
}