using AutoMapper;
using FlowerShop.Domain;

namespace FlowerShop.Application
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // User mappings
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => UserRole.Customer));

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            // Category mappings
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();

            // Flower mappings
            CreateMap<FlowerCreateDTO, Flower>();
            CreateMap<FlowerUpdateDTO, Flower>();
            CreateMap<Flower, FlowerDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "N/A"))
                .ForMember(dest => dest.FlowerImages, opt => opt.MapFrom(src => src.FlowerImages));

            // FlowerImage mappings
            CreateMap<FlowerImage, FlowerImageDTO>();

            // Cart mappings
            CreateMap<CartCreateDTO, Cart>();
            CreateMap<Cart, CartDTO>();

            // CartItem mappings
            CreateMap<CartItemCreateDTO, CartItem>()
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore());
            CreateMap<CartItemUpdateDTO, CartItem>();
            CreateMap<CartItem, CartItemDTO>()
                 .ForMember(dest => dest.FlowerName,
                     opt => opt.MapFrom(src => src.Flower != null ? src.Flower.FlowerName : string.Empty))
                 .ForMember(dest => dest.ImageUrl,
                     opt => opt.MapFrom(src =>
                         src.Flower != null && src.Flower.FlowerImages != null
                             ? src.Flower.FlowerImages
                                 .Select(img => img.Url)
                                 .FirstOrDefault()
                             : null));

        }
    }
}
