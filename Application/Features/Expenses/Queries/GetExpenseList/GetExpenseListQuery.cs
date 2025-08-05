using MediatR;
using Core.Utilities.Results;
using Application.Features.Expenses.Dtos;
using System.Collections.Generic;

namespace Application.Features.Expenses.Queries.GetExpenseList
{
    public class GetExpenseListQuery : IRequest<Response<List<ExpenseDto>>>
    {
    }
}
