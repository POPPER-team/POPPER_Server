using AutoMapper;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Helpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Guid, opt => opt.Ignore());
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString()))
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<NewUserDto, User>();
        CreateMap<User, NewUserDto>();

        CreateMap<UserDetailsDto, User>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)));
        CreateMap<User, UserDetailsDto>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString()));

        CreateMap<UserLoginDto, User>();
        CreateMap<User, UserLoginDto>();
    }
}