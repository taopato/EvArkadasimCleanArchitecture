using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Application.Features.Houses.Queries.GetSpendingOverview
{
    public class GetSpendingOverviewQueryHandler
        : IRequestHandler<GetSpendingOverviewQuery, SpendingOverviewDto>
    {
        private readonly IBillReadRepository _billRead;
        private readonly ILedgerLineRepository _ledgerRepo;

        public GetSpendingOverviewQueryHandler(
            IBillReadRepository billRead,
            ILedgerLineRepository ledgerRepo)
        {
            _billRead = billRead;
            _ledgerRepo = ledgerRepo;
        }

        public async Task<SpendingOverviewDto> Handle(GetSpendingOverviewQuery q, CancellationToken ct)
        {
            var to = q.To?.Date ?? DateTime.UtcNow.Date;
            var from = q.From?.Date ?? to.AddDays(-90);

            var dto = new SpendingOverviewDto
            {
                Range = new SpendingOverviewDto.RangeInfo { From = from, To = to }
            };

            // 1) Faturalar: Finalized + CreatedAt aralığı
            var bills = await _billRead.Query()
                .Where(b => b.HouseId == q.HouseId &&
                            b.Status == BillStatus.Finalized &&
                            b.CreatedAt.Date >= from && b.CreatedAt.Date <= to)
                .Select(b => new { b.UtilityType, b.Amount })
                .ToListAsync(ct);

            dto.UtilitiesTotals = bills
                .GroupBy(x => x.UtilityType.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            // 2) Açık borç/alacaklar (LedgerLine)
            var lines = await _ledgerRepo.GetListAsync(
                l => l.HouseId == q.HouseId && l.IsActive && !l.IsClosed, ct);

            var debts = lines
                .Select(l => new { l.FromUserId, l.ToUserId, Remaining = l.Amount - l.PaidAmount })
                .ToList();

            dto.OpenBalances = debts
                .GroupBy(x => x.FromUserId)
                .Select(g => new SpendingOverviewDto.UserOpenBalance
                {
                    UserId = g.Key,
                    DebtOpen = g.Sum(v => v.Remaining),
                    CreditOpen = debts.Where(d => d.ToUserId == g.Key).Sum(v => v.Remaining)
                })
                .ToList();

            // 3) Diğer bölümler (şimdilik boş)
            dto.PaymentsByUser = new();
            dto.GeneralExpenses = new();
            dto.RecentExpenses = new();

            // 4) Son faturalar
            dto.RecentBills = await _billRead.Query()
                .Where(b => b.HouseId == q.HouseId)
                .OrderByDescending(b => b.CreatedAt)
                .Take(q.RecentLimit)
                .Select(b => new SpendingOverviewDto.RecentBillItem
                {
                    BillId = b.Id,
                    UtilityType = (int)b.UtilityType,
                    Month = b.Month,
                    Amount = b.Amount
                })
                .ToListAsync(ct);

            return dto;
        }
    }
}
