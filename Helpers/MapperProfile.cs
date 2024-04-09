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

    }
}