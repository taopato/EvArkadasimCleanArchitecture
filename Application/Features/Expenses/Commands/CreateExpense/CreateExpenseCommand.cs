using MediatR;
using Application.Features.Expenses.Dtos;
using Domain.Enums; // ✅ Enum'u kullanabilmek için eklenmeli
using System;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommand : IRequest<CreatedExpenseResponseDto>
    {
        public string Tur { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public int HouseId { get; set; }
        public int OdeyenUserId { get; set; }
        public int KaydedenUserId { get; set; }
        public decimal OrtakHarcamaTutari { get; set; }
        public DateTime Date { get; set; }

        public PaylasimTuru PaylasimTuru { get; set; } // ✅ EKLENECEK ALAN

        public List<PersonalExpenseDto> SahsiHarcamalar { get; set; } = new();
    }
}
