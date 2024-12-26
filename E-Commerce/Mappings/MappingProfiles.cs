using AutoMapper;
using E_Commerce.DTOs.Category;
using E_Commerce.DTOs.Product;
using E_Commerce.Models;

namespace E_Commerce.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            MappingRulesProduct();
            MappingRulesCategory();
        }

        private void MappingRulesProduct()
        {
            CreateMap<CreateProductDTO, Product>()
                .ForMember(destination => destination.CategoryId,
                options => options.MapFrom(source => source.CategoryId))
                .ForMember(destination => destination.Category,
                option => option.Ignore());

            CreateMap<Product, ProductDTO>();
        }

        private void MappingRulesCategory()
        {
            CreateMap<Category, CategoryDTO>();
        }
    }
}
