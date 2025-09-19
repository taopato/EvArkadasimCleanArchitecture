using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Services.Repositories;
using Domain.Enums;

namespace Application.Features.Bills.Commands.FinalizeBill
{
    public class FinalizeBillCommandHandler : IRequestHandler<FinalizeBillCommand, bool>
    {
        private readonly IUtilityBillRepository _billRepo;

        public FinalizeBillCommandHandler(IUtilityBillRepository billRepo)
        {
            _billRepo = billRepo;
        }

        public async Task<bool> Handle(FinalizeBillCommand request, CancellationToken ct)
        {
            var bill = await _billRepo.GetAsync(request.BillId)
                        ?? throw new System.Exception("Bill not found.");

            if (bill.ResponsibleUserId != request.RequestUserId)
                throw new System.Exception("Sadece sorumlu kişi finalize edebilir.");

            if (bill.Status == BillStatus.Finalized)
                return true;

            // LedgerEntry: her pay için (kendi kendine kayıt yazma)
            var entries = bill.Shares
                .Where(s => s.UserId != bill.ResponsibleUserId && s.ShareAmount > 0)
                .Select(s => new Domain.Entities.LedgerEntry
                {
                    HouseId = bill.HouseId,
                    FromUserId = s.UserId,
                    ToUserId = bill.ResponsibleUserId,
                    Amount = s.ShareAmount,
                    PaidAmount = 0m,
                    IsClosed = false,
                    BillId = bill.Id,
                    UtilityType = bill.UtilityType,
                    Month = bill.Month,
                    Note = $"Bill #{bill.Id} {bill.UtilityType} {bill.Month}"
                }).ToList();

            if (entries.Any())

            bill.Status = BillStatus.Finalized;
            bill.FinalizedAt = System.DateTime.UtcNow;
            await _billRepo.UpdateAsync(bill);

            return true;
        }
    }
}
