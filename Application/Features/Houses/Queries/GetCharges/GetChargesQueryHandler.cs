using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Application.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Expenses.Queries.GetCharges
{
    public class GetChargesQueryHandler
        : IRequestHandler<GetChargesQuery, List<ChargeCycleDto>>
    {
        private readonly IChargeCycleRepository _cycleRepo;
        private readonly IRecurringChargeRepository _recRepo;

        public GetChargesQueryHandler(
            IChargeCycleRepository cycleRepo,
            IRecurringChargeRepository recRepo)
        {
            _cycleRepo = cycleRepo;
            _recRepo = recRepo;
        }

        public async Task<List<ChargeCycleDto>> Handle(GetChargesQuery request, CancellationToken ct)
        {
            // JOIN ile çekiyoruz; ToString YOK, string eşitliği var.
            var q =
                from c in _cycleRepo.Query()
                join r in _recRepo.Query() on c.ContractId equals r.Id
                where r.HouseId == request.HouseId
                select new { c, r };

            if (!string.IsNullOrWhiteSpace(request.Period))
                q = q.Where(x => x.c.Period == request.Period);

            var list = await q.Select(x => new ChargeCycleDto
            {
                CycleId = x.c.Id,
                ContractId = x.c.ContractId,
                HouseId = x.r.HouseId,
                Period = x.c.Period,
                Status = (int)x.c.Status,
                TotalAmount = x.c.TotalAmount,
                FundedAmount = x.c.FundedAmount,
                Type = (int)x.r.Type,
                SplitPolicy = (int)x.r.SplitPolicy,
                FixedAmount = x.r.FixedAmount,
                PayerUserId = x.r.PayerUserId
            }).ToListAsync(ct);

            return list;
        }
    }
}
