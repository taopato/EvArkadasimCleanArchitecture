using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Services.Repositories;
using Application.Features.Payments.Commands.CreatePayment; // <= VAR OLAN CreatePayment pipeline'ını kullanıyoruz

namespace Application.Features.Payments.Commands.AddPaymentWithAllocations
{
    public class AddPaymentWithAllocationsCommandHandler
        : IRequestHandler<AddPaymentWithAllocationsCommand, AddPaymentWithAllocationsResult>
    {
        private readonly IMediator _mediator;                            // Payment eklemek için
        private readonly IPaymentAllocationRepository _allocationRepo;   // PaymentAllocation yazacağız
        private readonly ILedgerEntryRepository _ledgerRepo;             // Ledger kapatacağız

        public AddPaymentWithAllocationsCommandHandler(
            IMediator mediator,
            IPaymentAllocationRepository allocationRepo,
            ILedgerEntryRepository ledgerRepo)
        {
            _mediator = mediator;
            _allocationRepo = allocationRepo;
            _ledgerRepo = ledgerRepo;
        }

        public async Task<AddPaymentWithAllocationsResult> Handle(AddPaymentWithAllocationsCommand request, CancellationToken ct)
        {
            var m = request.Model;

            if (m.Allocations == null || m.Allocations.Count == 0)
                throw new Exception("En az bir allocation gerekli.");

            var allocTotal = m.Allocations.Sum(a => a.Amount);
            if (allocTotal != m.Amount)
                throw new Exception("Allocations toplamı payment amount ile aynı olmalı.");

            // LedgerEntry'leri çek, doğrula
            var entries = await Task.WhenAll(m.Allocations.Select(async a =>
            {
                var le = await _ledgerRepo.GetByIdAsync(a.LedgerEntryId)
                         ?? throw new Exception($"LedgerEntry #{a.LedgerEntryId} bulunamadı.");

                if (le.HouseId != m.HouseId) throw new Exception($"LedgerEntry #{a.LedgerEntryId} başka evde.");
                if (le.ToUserId != m.ToUserId) throw new Exception($"LedgerEntry #{a.LedgerEntryId} alacaklı uyuşmuyor.");
                if (le.FromUserId != m.PayerUserId) throw new Exception($"LedgerEntry #{a.LedgerEntryId} borçlu uyuşmuyor.");

                var remaining = le.Amount - le.PaidAmount;
                if (a.Amount <= 0 || a.Amount > remaining)
                    throw new Exception($"LedgerEntry #{a.LedgerEntryId} için geçersiz allocation: {a.Amount} (kalan: {remaining}).");

                return le;
            }));

            // 1) Ödeme kaydını VAR OLAN komut ile oluştur (şema bağımlılığı yok)
            var created = await _mediator.Send(new CreatePaymentCommand
            {
                HouseId = m.HouseId,
                BorcluUserId = m.PayerUserId,     // borçlu = ödeyen
                AlacakliUserId = m.ToUserId,        // alacaklı
                Tutar = m.Amount,
                // OdemeTarihi / PaymentMethod / DekontUrl / Aciklama opsiyonel
                OdemeTarihi = DateTime.UtcNow,
                PaymentMethod = Domain.Enums.PaymentMethod.BankTransfer,
                Aciklama = m.Note,
                DekontUrl = null
            }, ct);

            // 2) Allocations yaz
            var allocationEntities = m.Allocations.Select(a => new Domain.Entities.PaymentAllocation
            {
                PaymentId = created.Id,      // CreatePaymentCommand dönen dto'daki Id
                LedgerEntryId = a.LedgerEntryId,
                Amount = a.Amount
            }).ToList();

            await _allocationRepo.AddRangeAsync(allocationEntities);

            // 3) Ledger güncelle
            var result = new AddPaymentWithAllocationsResult { PaymentId = created.Id };

            for (int i = 0; i < m.Allocations.Count; i++)
            {
                var a = m.Allocations[i];
                var le = entries[i];

                le.PaidAmount += a.Amount;
                le.IsClosed = le.PaidAmount >= le.Amount;

                await _ledgerRepo.UpdateAsync(le);

                result.AppliedAllocations.Add(new AddPaymentWithAllocationsResult.AppliedItem
                {
                    LedgerEntryId = le.Id,
                    Applied = a.Amount,
                    RemainingOnEntry = le.Amount - le.PaidAmount,
                    Closed = le.IsClosed
                });
            }

            return result;
        }
    }
}
