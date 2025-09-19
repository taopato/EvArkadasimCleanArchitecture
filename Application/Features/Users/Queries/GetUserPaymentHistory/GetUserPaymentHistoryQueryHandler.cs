using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Services.Repositories;

namespace Application.Features.Users.Queries.GetUserPaymentHistory
{
    public class GetUserPaymentHistoryQueryHandler
        : IRequestHandler<GetUserPaymentHistoryQuery, UserPaymentHistoryDto>
    {
        private readonly ILedgerLineRepository _ledgerRepo;

        public GetUserPaymentHistoryQueryHandler(ILedgerLineRepository ledgerRepo)
        {
            _ledgerRepo = ledgerRepo;
        }

        public async Task<UserPaymentHistoryDto> Handle(GetUserPaymentHistoryQuery q, CancellationToken ct)
        {
            var dto = new UserPaymentHistoryDto();

            // 1) Açık kalemler (kullanıcı borcu/alacağı)
            var openList = await _ledgerRepo.GetListAsync(l =>
                    !l.IsClosed &&
                    (l.FromUserId == q.UserId || l.ToUserId == q.UserId) &&
                    (!q.HouseId.HasValue || l.HouseId == q.HouseId.Value),
                ct);

            dto.OpenBalances.DebtOpen = openList.Where(x => x.FromUserId == q.UserId).Sum(x => x.Amount - x.PaidAmount);
            dto.OpenBalances.CreditOpen = openList.Where(x => x.ToUserId == q.UserId).Sum(x => x.Amount - x.PaidAmount);

            // 2) Son hareketler (yalnız ledger – payments kısmı şimdilik boş)
            var recent = await _ledgerRepo.GetListAsync(l =>
                    (l.FromUserId == q.UserId || l.ToUserId == q.UserId) &&
                    (!q.HouseId.HasValue || l.HouseId == q.HouseId.Value),
                ct);

            dto.Recent = recent
                .OrderByDescending(l => l.CreatedAt)
                .Take(q.Limit)
                .Select(l => new UserPaymentHistoryDto.HistoryItem
                {
                    Type = "ledger",
                    Id = (int)l.Id,                       // <-- cast eklendi
                    Date = l.CreatedAt,
                    Amount = l.Amount,
                    ToUserId = (int)l.ToUserId,           // <-- cast eklendi
                    Role = ((int)l.FromUserId == q.UserId) ? "debt" : "credit", // <-- kıyas cast
                    Remaining = l.Amount - l.PaidAmount,
                    Closed = l.IsClosed,
                    Note = null
                })
                .ToList();

            return dto;
        }
    }
}
