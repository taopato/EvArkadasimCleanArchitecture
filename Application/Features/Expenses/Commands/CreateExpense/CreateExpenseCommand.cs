using Application.Features.Expenses.Dtos;
using MediatR;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommand : IRequest<CreatedExpenseDto>
    {
        public string Description { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public int KaydedenUserId { get; set; }
        public int OdeyenUserId { get; set; }
    }
}
