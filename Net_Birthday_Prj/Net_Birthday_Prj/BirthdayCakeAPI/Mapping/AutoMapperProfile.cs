using AutoMapper;
using BusinessLogic.DTOs.Categories;
using BusinessLogic.DTOs.Products;
using BusinessLogic.DTOs.Users;
using BusinessLogic.Entities;

namespace BirthdayCakeAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
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

            // User → UserDTO (để trả trong LoginResponse)
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Email, m => m.MapFrom(s => s.Email))
                .ForMember(d => d.FullName, m => m.MapFrom(s => s.FullName))
                .ForMember(d => d.Roles, m => m.Ignore()); // fill ở controller/service
        }
    }
}
