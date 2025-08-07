using AutoMapper;
using Domain.Entities;
using Application.Features.Expenses.Dtos;
using Application.Features.Expenses.Commands.CreateExpense;

namespace Application.Features.Expenses.Profiles
{
    public class ExpenseMappingProfile : Profile
    {
        public ExpenseMappingProfile()
        {
            CreateMap<Expense, ExpenseDto>().ReverseMap();
            CreateMap<Expense, CreatedExpenseDto>().ReverseMap();
            CreateMap<CreateExpenseCommand, Expense>()
                .ForMember(dest => dest.OrtakHarcamaTutari, opt => opt.Ignore())
                .ReverseMap();

            // ⬇ Eksik olan map burada
            CreateMap<Expense, ExpenseListDto>()
                .ForMember(dest => dest.Tur, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.KayitTarihi, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.OdeyenKullaniciAdi, opt => opt.MapFrom(src => src.OdeyenUser.FirstName + " " + src.OdeyenUser.LastName))
                .ForMember(dest => dest.KaydedenKullaniciAdi, opt => opt.MapFrom(src => src.KaydedenUser.FirstName + " " + src.KaydedenUser.LastName));

        }
    }
}
