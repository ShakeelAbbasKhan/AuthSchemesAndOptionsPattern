using AuthSchemesAndOptionsPattern.Dtos;
using AuthSchemesAndOptionsPattern.Model;
using AutoMapper;

namespace AuthSchemesAndOptionsPattern.MapperProfile
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, Product>().ReverseMap();
            CreateMap<UpdateProductDto, Product>().ReverseMap();

        }
    }
}
