using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Features.Houses.Dtos;
using Application.Services.Repositories;
using MediatR;

namespace Application.Features.Houses.Queries.GetUserDebts
{
    public class GetUserDebtsQueryHandler : IRequestHandler<GetUserDebtsQuery, GetUserDebtsDto>
    {
        private readonly ILedgerLineRepository _ledgerRepo;

        public GetUserDebtsQueryHandler(ILedgerLineRepository ledgerRepo)
        {
            _ledgerRepo = ledgerRepo;
        }

        public async Task<GetUserDebtsDto> Handle(GetUserDebtsQuery request, CancellationToken ct)
        {
            var lines = await _ledgerRepo.GetListAsync(l => l.HouseId == request.HouseId && l.IsActive);

            // long tiplerine göre netleme (entity alanları long ise cast hatası olmasın)
            var pairForward = new Dictionary<(long from, long to), decimal>();
            var receivableByUser = new Dictionary<long, decimal>();
            var payableByUser = new Dictionary<long, decimal>();

            foreach (var l in lines)
            {
                var key = ((long)l.FromUserId, (long)l.ToUserId);
                if (!pairForward.ContainsKey(key)) pairForward[key] = 0m;
                pairForward[key] += l.Amount;

                var fu = (long)l.FromUserId;
                var tu = (long)l.ToUserId;

                if (!payableByUser.ContainsKey(fu)) payableByUser[fu] = 0m;
                if (!receivableByUser.ContainsKey(tu)) receivableByUser[tu] = 0m;

                payableByUser[fu] += l.Amount;
                receivableByUser[tu] += l.Amount;
            }

            var usersInHouse = new HashSet<long>(
                lines.Select(l => (long)l.FromUserId).Concat(lines.Select(l => (long)l.ToUserId))
            );

            var netPairs = new List<GetUserDebtsDto.PairDebtDto>();
            foreach (var a in usersInHouse)
            {
                foreach (var b in usersInHouse)
                {
                    if (a >= b) continue; // her çifti bir kez

                    var ab = pairForward.TryGetValue((a, b), out var abSum) ? abSum : 0m;
                    var ba = pairForward.TryGetValue((b, a), out var baSum) ? baSum : 0m;
                    var net = ab - ba;

                    if (net > 0)
                        netPairs.Add(new GetUserDebtsDto.PairDebtDto { FromUserId = (int)a, ToUserId = (int)b, NetAmount = net });
                    else if (net < 0)
                        netPairs.Add(new GetUserDebtsDto.PairDebtDto { FromUserId = (int)b, ToUserId = (int)a, NetAmount = -net });
                }
            }

            var totals = usersInHouse
                .Select(u =>
                {
                    var rec = receivableByUser.TryGetValue(u, out var r) ? r : 0m;
                    var pay = payableByUser.TryGetValue(u, out var p) ? p : 0m;
                    return new GetUserDebtsDto.UserTotalDto
                    {
                        UserId = (int)u,
                        Receivable = rec,
                        Payable = pay,
                        Net = rec - pay
                    };
                })
                .OrderByDescending(t => t.Net)
                .ToList();

            if (request.UserId.HasValue)
            {
                var uid = request.UserId.Value;
                netPairs = netPairs.Where(p => p.FromUserId == uid || p.ToUserId == uid).ToList();
                totals = totals.Where(t => t.UserId == uid).ToList();
            }

            return new GetUserDebtsDto
            {
                HouseId = request.HouseId,
                Pairs = netPairs,
                Totals = totals
            };
        }
    }
}
