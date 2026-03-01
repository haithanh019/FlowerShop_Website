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
        }
    }
}
