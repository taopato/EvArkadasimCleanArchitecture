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
        }
    }
}
