using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries.GetUserPaymentHistory
{
    public class GetUserPaymentHistoryQueryHandler
        : IRequestHandler<GetUserPaymentHistoryQuery, UserPaymentHistoryDto>
    {
        private readonly ILedgerReadRepository _ledgerRead;

        public GetUserPaymentHistoryQueryHandler(ILedgerReadRepository ledgerRead)
        {
            _ledgerRead = ledgerRead;
        }

        public async Task<UserPaymentHistoryDto> Handle(GetUserPaymentHistoryQuery q, CancellationToken ct)
        {
            var dto = new UserPaymentHistoryDto();

            // Open balances
            var openEntries = _ledgerRead.Query()
                .Where(l => !l.IsClosed && (l.FromUserId == q.UserId || l.ToUserId == q.UserId));

            if (q.HouseId.HasValue)
                openEntries = openEntries.Where(l => l.HouseId == q.HouseId.Value);

            var openList = await openEntries
                .Select(l => new { l.FromUserId, l.ToUserId, Remaining = l.Amount - l.PaidAmount })
                .ToListAsync(ct);

            dto.OpenBalances.DebtOpen = openList.Where(x => x.FromUserId == q.UserId).Sum(x => x.Remaining);
            dto.OpenBalances.CreditOpen = openList.Where(x => x.ToUserId == q.UserId).Sum(x => x.Remaining);

            // Recent ledger (user borç/alacak kalemleri) – payments bölümü şimdilik boş
            var lQuery = _ledgerRead.Query()
                .Where(l => l.FromUserId == q.UserId || l.ToUserId == q.UserId);
            if (q.HouseId.HasValue) lQuery = lQuery.Where(l => l.HouseId == q.HouseId.Value);

            dto.Recent = await lQuery
                .OrderByDescending(l => l.CreatedAt)
                .Take(q.Limit)
                .Select(l => new UserPaymentHistoryDto.HistoryItem
                {
                    Type = "ledger",
                    Id = l.Id,
                    Date = l.CreatedAt,
                    Amount = l.Amount,
                    ToUserId = l.ToUserId,
                    Role = (l.FromUserId == q.UserId) ? "debt" : "credit",
                    Remaining = l.Amount - l.PaidAmount,
                    Closed = l.IsClosed,
                    Note = l.Note
                }).ToListAsync(ct);

            return dto;
        }
    }
}
