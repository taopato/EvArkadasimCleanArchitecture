using Application.Features.Expenses.Dtos;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Expenses.Queries.GetExpenseList
{
    public class GetExpenseListQuery : IRequest<List<ExpenseDto>>
    {
    }
}
