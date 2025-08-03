using AutoMapper;
using MediatR;
using Application.Features.Expenses.Dtos;
using Application.Services.Repositories;
using System.Collections.Generic;

namespace Application.Features.Expenses.Queries.GetExpenseList
{
    public class GetExpenseListQueryHandler : IRequestHandler<GetExpenseListQuery, List<ExpenseDto>>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public GetExpenseListQueryHandler(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<List<ExpenseDto>> Handle(GetExpenseListQuery request, CancellationToken cancellationToken)
        {
            var list = await _expenseRepository.GetAllAsync();
            return _mapper.Map<List<ExpenseDto>>(list);
        }
    }
}
