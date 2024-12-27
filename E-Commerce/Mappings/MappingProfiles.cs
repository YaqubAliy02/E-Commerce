using AutoMapper;
using E_Commerce.DTOs.Category;
using E_Commerce.DTOs.Customer;
using E_Commerce.DTOs.Oder;
using E_Commerce.DTOs.OrderItem;
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
            MappingRulesCustomer();
            MappingRulesOrder();
            MappingRulesOrderItem();
        }

        private void MappingRulesOrderItem()
        {
            CreateMap<CreateOrderItemDTO, OrderItem>()
                .ForMember(destination => destination.Order,
                option => option.Ignore())
                .ForMember(destination => destination.Product,
                option => option.Ignore());

            CreateMap<OrderItem, OrderItemDTO>();
        }

        private void MappingRulesOrder()
        {
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(destination => destination.Customer,
                option => option.Ignore())
                .ForMember(destination => destination.PaymentStatus, option => option
                .MapFrom(source => source.PaymentStatus));

            CreateMap<Order, OrderDTO>()
                .ForMember(destination => destination.Customer,
                option => option.MapFrom(source => source.Customer))
                .ForMember(destination => destination.OrderItems,
                option => option.MapFrom(source => source.OrderItems));

        }

        private void MappingRulesCustomer()
        {
            CreateMap<CreateCustomerDTO, Customer>();
            CreateMap<Customer, CustomerDTO>();
        }

        private void MappingRulesProduct()
        {
            CreateMap<CreateProductDTO, Product>()
                .ForMember(destination => destination.CategoryId,
                options => options.MapFrom(source => source.CategoryId))
                .ForMember(destination => destination.Category,
                option => option.Ignore())
                .ForMember(destination => destination.ImageUrl, 
                option => option.MapFrom(source => source.ImageUrl));

            CreateMap<Product, ProductDTO>();
        }
            
        private void MappingRulesCategory()
        {
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<UpdateCategoryDTO, Category>();
        }
    }
}
