using MediatR;
using Application.Services.Repositories;
using Domain.Entities;

namespace Application.Features.Expenses.Commands.DeleteExpense
{
    public class DeleteExpenseCommandHandler
        : IRequestHandler<DeleteExpenseCommand, Unit>
    {
        private readonly IExpenseRepository _repo;
        public DeleteExpenseCommandHandler(IExpenseRepository repo) => _repo = repo;

        public async Task<Unit> Handle(
            DeleteExpenseCommand request,
            CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(request.ExpenseId)
                    ?? throw new KeyNotFoundException("Expense not found");
            await _repo.DeleteAsync(e);
            return Unit.Value;
        }
    }
}