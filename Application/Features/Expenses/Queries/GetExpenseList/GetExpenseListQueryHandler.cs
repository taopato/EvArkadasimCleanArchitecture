using AutoMapper;
using MediatR;
using Core.Utilities.Results;
using Application.Services.Repositories; // ✅ Doğru namespace
using Application.Features.Expenses.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Expenses.Queries.GetExpenseList
{
    public class GetExpenseListQueryHandler
        : IRequestHandler<GetExpenseListQuery, Response<List<ExpenseDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IExpenseRepository _expenseRepository;

        public GetExpenseListQueryHandler(
            IMapper mapper,
            IExpenseRepository expenseRepository)
        {
            _mapper = mapper;
            _expenseRepository = expenseRepository;
        }

        public async Task<Response<List<ExpenseDto>>> Handle(
            GetExpenseListQuery request,
            CancellationToken cancellationToken)
        {
            var entities = await _expenseRepository.GetListAsync();
            var dtos = _mapper.Map<List<ExpenseDto>>(entities);
            return new Response<List<ExpenseDto>>(dtos, true);
        }
    }
}
