using AutoMapper;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Helpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<NewUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());
        CreateMap<UserLoginDto, User>();


        CreateMap<UserDetailsDto, User>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)))
            .ForMember(dest => dest.Guid, opt => opt.Ignore());
        CreateMap<User, UserDetailsDto>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString()));

        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Guid, opt => opt.Ignore());
    }
}