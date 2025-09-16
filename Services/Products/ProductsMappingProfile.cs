// ProductsMappingProfile.cs
using App.Repositories.Products;
using App.Services.Products.Create;
using App.Services.Products.Update;
using AutoMapper;

namespace App.Services.Products
{
    public class ProductsMappingProfile : Profile
    {
        public ProductsMappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<CreateProductRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore());

            CreateMap<UpdateProductRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore());
        }
    }
}
