// Application/Features/Expenses/Commands/CreateExpense/CreateExpenseCommand.cs
using MediatR;
using Application.Features.Expenses.Dtos;
using Domain.Enums;
using System;
using System.Collections.Generic;

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

        public PaylasimTuru PaylasimTuru { get; set; }  // kullanılmasa da dursun
        public List<PersonalExpenseDto> SahsiHarcamalar { get; set; } = new();
    }
}
