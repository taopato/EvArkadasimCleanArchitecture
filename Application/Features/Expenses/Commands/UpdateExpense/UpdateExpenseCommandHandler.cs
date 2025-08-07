using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Expenses.Commands.UpdateExpense;
using Application.Features.Expenses.Dtos;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Expenses.Commands.UpdateExpense
{
    public class UpdateExpenseCommandHandler
        : IRequestHandler<UpdateExpenseCommand, Unit>
    {
        private readonly IExpenseRepository _repo;
        private readonly IPersonalExpenseRepository _personalRepo;
        private readonly IShareRepository _shareRepo;
        private readonly IHouseMemberRepository _houseMemberRepo;

        public UpdateExpenseCommandHandler(
            IExpenseRepository repo,
            IPersonalExpenseRepository personalRepo,
            IShareRepository shareRepo,
            IHouseMemberRepository houseMemberRepo)
        {
            _repo = repo;
            _personalRepo = personalRepo;
            _shareRepo = shareRepo;
            _houseMemberRepo = houseMemberRepo;
        }

        public async Task<Unit> Handle(
            UpdateExpenseCommand request,
            CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(request.ExpenseId)
                    ?? throw new KeyNotFoundException("Expense not found");

            // 1) Temel güncelleme
            e.Description = request.Dto.Tur;
            e.Tutar = request.Dto.Tutar;
            e.OrtakHarcamaTutari = request.Dto.OrtakHarcamaTutari;
            await _repo.UpdateAsync(e);

            // 2) Şahsi harcamaları sil + yeniden ekle
            foreach (var pe in e.PersonalExpenses.ToList())
                await _personalRepo.DeleteAsync(pe);

            foreach (var peDto in request.Dto.SahsiHarcamalar)
            {
                await _personalRepo.AddAsync(new PersonalExpense
                {
                    ExpenseId = e.Id,
                    UserId = peDto.UserId,
                    Tutar = peDto.Tutar
                });
            }

            // 3) Share’leri sil + yeniden ekle
            foreach (var s in e.Shares.ToList())
                await _shareRepo.DeleteAsync(s);

            var members = await _houseMemberRepo.GetByHouseIdAsync(e.HouseId);
            foreach (var m in members)
                await _shareRepo.AddAsync(new Share
                {
                    ExpenseId = e.Id,
                    UserId = m.UserId,
                    PaylasimTutar = request.Dto.OrtakHarcamaTutari
                });

            // ✅ SaveChanges unutulmasın
            await _shareRepo.SaveChangesAsync();
            await _personalRepo.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
