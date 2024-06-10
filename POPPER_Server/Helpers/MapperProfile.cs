using AutoMapper;
using POPPER_Server.Dtos;
using POPPER_Server.Models;

namespace POPPER_Server.Helpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<NewUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)));
        CreateMap<UserLoginDto, User>();


        CreateMap<UserDetailsDto, User>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)))
            .ForMember(dest => dest.Guid, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created.ToString()));
        CreateMap<User, UserDetailsDto>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString()));

        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Guid, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore());

        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.UserGuid, opt => opt.MapFrom(src => src.User.Guid))
            .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.viewCount, opt => opt.MapFrom(src => src.Views.Count))
            .ForMember(dest => dest.SavedCount, opt => opt.MapFrom(src => src.Saveds.Count))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration.ToString("g")));

        CreateMap<NewPostDto, Post>()
            .ForMember(dest => dest.Guid, opt => opt.Ignore())
            .ForMember(dest => dest.Duration, opt => opt.Ignore())
            .ForMember(dest => dest.Steps, opt => opt.Ignore())
            .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
    }
}
