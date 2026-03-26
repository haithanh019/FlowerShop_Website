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
            CreateMap<FlowerCreateDTO, Flower>()
                .ForMember(dest => dest.FlowerImages, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<FlowerUpdateDTO, Flower>()
                .ForMember(dest => dest.FlowerImages, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<Flower, FlowerDTO>()
                .ForMember(dest => dest.CategoryName,
                 opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.FlowerImages, opt => opt.MapFrom(src => src.FlowerImages));

            // FlowerImage mappings
            CreateMap<FlowerImage, FlowerImageDTO>();
            CreateMap<FlowerImageDTO, FlowerImage>()
                .ForMember(dest => dest.Flower, opt => opt.Ignore());

            // Cart mappings
            CreateMap<Cart, CartDTO>()
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems));
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

            // Address mappings
            CreateMap<Address, AddressDTO>();
            CreateMap<AddressCreateDTO, Address>();
            CreateMap<AddressUpdateDTO, Address>();

            // Order mappings
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderStatus,
                    opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.OrderItems,
                    opt => opt.MapFrom(src => src.OrderItems));

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.FlowerName,
                    opt => opt.MapFrom(src => src.Flower != null ? src.Flower.FlowerName : string.Empty))
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src =>
                        src.Flower != null && src.Flower.FlowerImages != null
                            ? src.Flower.FlowerImages.Select(img => img.Url).FirstOrDefault()
                            : null));

            // Payment mappings
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatus,
                    opt => opt.MapFrom(src => src.PaymentStatus.ToString()));

        }
    }
}
