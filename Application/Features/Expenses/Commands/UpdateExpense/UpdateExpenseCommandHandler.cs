using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
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
        private readonly ILedgerLineRepository _ledgerRepo;

        public UpdateExpenseCommandHandler(
            IExpenseRepository repo,
            IPersonalExpenseRepository personalRepo,
            IShareRepository shareRepo,
            IHouseMemberRepository houseMemberRepo,
            ILedgerLineRepository ledgerRepo)
        {
            _repo = repo;
            _personalRepo = personalRepo;
            _shareRepo = shareRepo;
            _houseMemberRepo = houseMemberRepo;
            _ledgerRepo = ledgerRepo;
        }

        public async Task<Unit> Handle(UpdateExpenseCommand request, CancellationToken ct)
        {
            var e = await _repo.GetByIdAsync(request.ExpenseId)
                    ?? throw new KeyNotFoundException("Expense not found");

            // ❗ Drift koruması:
            // Bu harcamaya ait, yayına çıkmış ve kısmen/ tamamen ödenmiş borç satırı varsa
            // tutar/paylaşım gibi finansal alanları değiştirmiyoruz.
            var paidLines = await _ledgerRepo.GetListAsync(
                l => l.ExpenseId == e.Id && l.IsActive && l.PaidAmount > 0m,
                ct);

            if (paidLines.Any())
                throw new InvalidOperationException(
                    "Bu harcama için ödenmiş borç satırları bulunduğu için finansal alanlar (Tutar/Ortak/Sahsi paylaşım) güncellenemez. " +
                    "Yalnız açıklama/not gibi finansal olmayan alanları güncelleyiniz.");

            // 1) Temel güncelleme
            // e.Description = request.Dto.Tur;  // ❌ yanlış: notu başlıkla ezmeyelim
            e.Tur = request.Dto.Tur;
            e.Tutar = request.Dto.Tutar;
            e.OrtakHarcamaTutari = request.Dto.OrtakHarcamaTutari;

            // 🔹 Not/Açıklama: FE "Aciklama", "Note" veya "Description" gönderebilir
            var incomingDesc = request.Dto.Aciklama ?? request.Dto.Note ?? request.Dto.Description;
            if (!string.IsNullOrWhiteSpace(incomingDesc))
                e.Description = incomingDesc;

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

            // 3) Share’leri sil + yeniden ekle (mevcut mimariyi koruyoruz)
            foreach (var s in e.Shares.ToList())
                await _shareRepo.DeleteAsync(s);

            var members = await _houseMemberRepo.GetByHouseIdAsync(e.HouseId);
            foreach (var m in members)
            {
                await _shareRepo.AddAsync(new Share
                {
                    ExpenseId = e.Id,
                    UserId = m.UserId,
                    PaylasimTutar = request.Dto.OrtakHarcamaTutari
                });
            }

            // SaveChanges
            await _shareRepo.SaveChangesAsync();
            await _personalRepo.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
