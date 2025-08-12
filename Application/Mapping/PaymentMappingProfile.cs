using Application.Features.Payments.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.PaymentMethod, m => m.MapFrom(s => s.PaymentMethod.ToString()))
                .ForMember(d => d.Status, m => m.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.BorcluUserName, m => m.MapFrom(s =>
                    s.BorcluUser != null ? (s.BorcluUser.FirstName + " " + s.BorcluUser.LastName).Trim() : string.Empty
                ));
        }
    }
}
