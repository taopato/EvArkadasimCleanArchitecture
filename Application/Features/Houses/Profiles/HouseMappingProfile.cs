using AutoMapper;
using Domain.Entities;
using Application.Features.Houses.Dtos;

namespace Application.Features.Houses.Profiles
{
    public class HouseMappingProfile : Profile
    {
        public HouseMappingProfile()
        {
            CreateMap<House, HouseDto>().ReverseMap();
            CreateMap<House, CreatedHouseDto>().ReverseMap();
            CreateMap<House, HouseDetailDto>()
                .ForMember(dest => dest.Members,
                           opt => opt.MapFrom(src => src.HouseMembers));

            CreateMap<HouseMember, MemberDto>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src =>
                               src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Email,
                           opt => opt.MapFrom(src => src.User.Email));
            // UserId ve JoinedDate gibi basit alanlar otomatik eşlenir
        }
    }
}
