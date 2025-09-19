using AutoMapper;
using MediatR;
using Core.Utilities.Results;
using Application.Services.Repositories;
using Application.Features.Expenses.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
            var entities = (await _expenseRepository.GetListAsync()).Where(x => x.IsActive).ToList();
            var dtos = _mapper.Map<List<ExpenseDto>>(entities);

            for (int i = 0; i < dtos.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(dtos[i].Description))
                    dtos[i].Description = entities[i].Note ?? string.Empty;
            }

            return new Response<List<ExpenseDto>>(dtos, true);
        }
    }
}
