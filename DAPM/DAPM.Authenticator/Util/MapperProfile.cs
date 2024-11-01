using AutoMapper;
using DAPM.Authenticator.Models;
using UtilLibrary;
namespace DAPM.Authenticator.Util
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegistrationDto, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.OrganizationName))
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId));

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.OrganizationName))
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId));


            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Name.ToString()));
        }
    }
}
