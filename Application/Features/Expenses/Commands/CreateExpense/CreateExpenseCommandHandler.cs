// Application/Features/Expenses/Commands/CreateExpense/CreateExpenseCommandHandler.cs

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Expenses.Commands.CreateExpense;
using Application.Features.Expenses.Dtos;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Expenses.Commands.CreateExpense
{
    public class CreateExpenseCommandHandler
        : IRequestHandler<CreateExpenseCommand, CreatedExpenseResponseDto>
    {
        private readonly IExpenseRepository _expenseRepo;
        private readonly IPersonalExpenseRepository _personalRepo;
        private readonly IShareRepository _shareRepo;
        private readonly IHouseMemberRepository _houseMemberRepo;

        public CreateExpenseCommandHandler(
            IExpenseRepository expenseRepo,
            IPersonalExpenseRepository personalRepo,
            IShareRepository shareRepo,
            IHouseMemberRepository houseMemberRepo)
        {
            _expenseRepo = expenseRepo;
            _personalRepo = personalRepo;
            _shareRepo = shareRepo;
            _houseMemberRepo = houseMemberRepo;
        }

        public async Task<CreatedExpenseResponseDto> Handle(
            CreateExpenseCommand request,
            CancellationToken cancellationToken)
        {
            // 1) Expense kaydı
            var exp = new Expense
            {
                Description = request.Tur,
                Tutar = request.Tutar,
                OrtakHarcamaTutari = request.OrtakHarcamaTutari,
                HouseId = request.HouseId,
                OdeyenUserId = request.OdeyenUserId,
                KaydedenUserId = request.KaydedenUserId,
                CreatedDate = System.DateTime.UtcNow
            };
            var addedExpense = await _expenseRepo.AddAsync(exp);

            // 2) Şahsi harcamalar ekle
            var personalDtos = new List<PersonalExpenseDto>();
            foreach (var pe in request.SahsiHarcamalar)
            {
                var peEntity = new PersonalExpense
                {
                    ExpenseId = addedExpense.Id,
                    UserId = pe.UserId,
                    Tutar = pe.Tutar
                };
                await _personalRepo.AddAsync(peEntity);
                personalDtos.Add(pe);
            }

            // 3) Share ekle
            var members = await _houseMemberRepo.GetByHouseIdAsync(request.HouseId);
            var shareDtos = new List<ShareDto>();
            foreach (var m in members)
            {
                var shareEntity = new Share
                {
                    ExpenseId = addedExpense.Id,
                    UserId = m.UserId,
                    PaylasimTutar = request.OrtakHarcamaTutari
                };
                await _shareRepo.AddAsync(shareEntity);
                shareDtos.Add(new ShareDto
                {
                    UserId = m.UserId,
                    PaylasimTutar = shareEntity.PaylasimTutar
                });
            }

            // 4) Response
            return new CreatedExpenseResponseDto
            {
                Id = addedExpense.Id,
                Tur = addedExpense.Description,
                Tutar = addedExpense.Tutar,
                OrtakHarcamaTutari = addedExpense.OrtakHarcamaTutari,
                PersonalExpenses = personalDtos,
                Shares = shareDtos
            };
        }
    }
}
