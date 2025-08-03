using Application.Features.Users.Commands.Create;
using Application.Features.Users.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Users.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreatedUserDto>().ReverseMap();
            CreateMap<CreateUserCommand, User>().ReverseMap();

        }
    }
}
