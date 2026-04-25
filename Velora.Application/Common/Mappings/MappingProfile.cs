using AutoMapper;
using Velora.Application.Products.Queries;
using Velora.Core.Entities;

namespace Velora.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
    }
}
