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
        private readonly ILedgerReadRepository _ledgerRead;

        public GetSpendingOverviewQueryHandler(
            IBillReadRepository billRead,
            ILedgerReadRepository ledgerRead)
        {
            _billRead = billRead;
            _ledgerRead = ledgerRead;
        }

        public async Task<SpendingOverviewDto> Handle(GetSpendingOverviewQuery q, CancellationToken ct)
        {
            var to = q.To?.Date ?? DateTime.UtcNow.Date;
            var from = q.From?.Date ?? to.AddDays(-90);

            var dto = new SpendingOverviewDto
            {
                Range = new SpendingOverviewDto.RangeInfo { From = from, To = to }
            };

            // Utilities toplamları (Finalized Bills, CreatedAt range)
            var bills = await _billRead.Query()
                .Where(b => b.HouseId == q.HouseId &&
                            b.Status == BillStatus.Finalized &&
                            b.CreatedAt.Date >= from && b.CreatedAt.Date <= to)
                .Select(b => new { b.UtilityType, b.Amount })
                .ToListAsync(ct);

            dto.UtilitiesTotals = bills
                .GroupBy(x => x.UtilityType.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

            // Open balances (ledger)
            var debts = await _ledgerRead.Query()
                .Where(l => l.HouseId == q.HouseId && !l.IsClosed)
                .Select(l => new { l.FromUserId, l.ToUserId, Remaining = l.Amount - l.PaidAmount })
                .ToListAsync(ct);

            dto.OpenBalances = debts
                .GroupBy(x => x.FromUserId)
                .Select(g => new SpendingOverviewDto.UserOpenBalance
                {
                    UserId = g.Key,
                    DebtOpen = g.Sum(v => v.Remaining),
                    CreditOpen = debts.Where(d => d.ToUserId == g.Key).Sum(v => v.Remaining)
                }).ToList();

            // Şimdilik paymentsByUser, generalExpenses, recentExpenses boş
            dto.PaymentsByUser = new();
            dto.GeneralExpenses = new();
            dto.RecentExpenses = new();

            // Recent bills
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
                }).ToListAsync(ct);

            return dto;
        }
    }
}
