using Application.Features.Expenses.Dtos;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, CreatedExpenseResponseDto>
    {
        private readonly IExpenseRepository _expenseRepo;
        private readonly IPersonalExpenseRepository _personalRepo;
        private readonly IShareRepository _shareRepo;

        public CreateExpenseCommandHandler(
            IExpenseRepository expenseRepo,
            IPersonalExpenseRepository personalRepo,
            IShareRepository shareRepo)
        {
            _expenseRepo = expenseRepo;
            _personalRepo = personalRepo;
            _shareRepo = shareRepo;
        }

        public async Task<CreatedExpenseResponseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            // 1) Expense kaydı
            var expense = new Expense
            {
                Description = request.Tur,
                Tutar = request.Tutar,
                OrtakHarcamaTutari = request.OrtakHarcamaTutari,
                HouseId = request.HouseId,
                OdeyenUserId = request.OdeyenUserId,
                KaydedenUserId = request.KaydedenUserId,
                CreatedDate = request.Date,
                Shares = new List<Share>(),
                PersonalExpenses = new List<PersonalExpense>()
            };

            var createdExpense = await _expenseRepo.AddAsync(expense);
            await _expenseRepo.SaveChangesAsync(); // ID oluşsun

            // 2) Share kayıtları
            var shareEntities = request.SahsiHarcamalar
                .Select(s => new Share
                {
                    ExpenseId = createdExpense.Id,
                    UserId = s.UserId,
                    PaylasimTutar = s.Tutar,
                    PaylasimTuru = request.PaylasimTuru // ✅ zorunlu enum alanı burada eklenmeli
                }).ToList();

            await _shareRepo.AddRangeAsync(shareEntities);
            await _shareRepo.SaveChangesAsync();

            // 3) Dönüş
            return new CreatedExpenseResponseDto
            {
                Id = createdExpense.Id,
                HouseId = createdExpense.HouseId,
                Description = createdExpense.Description,
                Tutar = createdExpense.Tutar
            };
        }
    }
}
