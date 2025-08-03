using AutoMapper;
using Domain.Entities;
using MediatR;
using Application.Features.Expenses.Dtos;
using Application.Services.Repositories;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, CreatedExpenseDto>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public CreateExpenseCommandHandler(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<CreatedExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            // 1) Ev grubundaki üye sayısını al
            var grupKisiSayisi = await _expenseRepository.GetHouseMemberCountAsync(request.KaydedenUserId);

            // 2) Command → Expense entity’sine map et
            var entity = _mapper.Map<Expense>(request);

            // 3) Ortak harcama tutarını entity üzerinde hesapla
            entity.OrtakHarcamaTutari = request.Tutar / grupKisiSayisi;

            // 4) Kaydet ve DTO’ya dönüştür
            var added = await _expenseRepository.AddAsync(entity);
            return _mapper.Map<CreatedExpenseDto>(added);
        }
    }
}
