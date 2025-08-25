using AutoMapper;
using BusinessLogic.DTOs.Carts;
using BusinessLogic.DTOs.Carts.CartItems;
using BusinessLogic.DTOs.Categories;
using BusinessLogic.DTOs.Orders;
using BusinessLogic.DTOs.Orders.OrderItemDTOs;
using BusinessLogic.DTOs.Products;
using BusinessLogic.DTOs.Users;
using BusinessLogic.Entities;

namespace BirthdayCakeAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User → UserDTO (để trả trong LoginResponse)
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Email, m => m.MapFrom(s => s.Email))
                .ForMember(d => d.FullName, m => m.MapFrom(s => s.FullName))
                .ForMember(d => d.Roles, m => m.Ignore()); // fill ở controller/service

            // Category
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();

            // Product
            CreateMap<Product, ProductDTO>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
            CreateMap<CreateProductDTO, Product>()
                .ForMember(d => d.ProductId, o => o.Ignore()); // Id do DB cấp
            CreateMap<UpdateProductDTO, Product>();

            // ShoppingCart
            CreateMap<ShoppingCart, ShoppingCartDTO>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
            CreateMap<CreateShoppingCartDTO, ShoppingCart>();
            CreateMap<UpdateShoppingCartDTO, ShoppingCart>();

            // CartItem
            CreateMap<CartItem, CartItemDTO>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));
            CreateMap<CreateCartItemDTO, CartItem>()
                .ForMember(d => d.UnitPrice, o => o.Ignore()); // set theo Product.Price
            CreateMap<UpdateCartItemDTO, CartItem>();

            // Order
            CreateMap<Order, OrderDTO>()
    .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
            CreateMap<CreateOrderFromCartDTO, Order>();       // chỉ map DeliveryAddress, CartId
            CreateMap<UpdateOrderStatusDTO, Order>();

            // Order Item
            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();
        }
    }
}
