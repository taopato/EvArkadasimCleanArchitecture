using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Charges.Dtos;
using Application.Services.Repositories;
using Core.Utilities.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Charges.Queries.GetChargesList
{
    public class GetChargesListQueryHandler
        : IRequestHandler<GetChargesListQuery, Response<List<ChargeCycleSummaryDto>>>
    {
        private readonly IRecurringChargeRepository _recRepo;
        private readonly IChargeCycleRepository _cycleRepo;
        private readonly IPaymentRepository _paymentRepo;

        public GetChargesListQueryHandler(
            IRecurringChargeRepository recRepo,
            IChargeCycleRepository cycleRepo,
            IPaymentRepository paymentRepo)
        {
            _recRepo = recRepo;
            _cycleRepo = cycleRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<Response<List<ChargeCycleSummaryDto>>> Handle(
            GetChargesListQuery q, CancellationToken ct)
        {
            var houseId = q.HouseId;
            var period = q.Period; // "YYYY-MM"

            var contracts = await _recRepo.Query()
                .Where(c => c.HouseId == houseId && c.IsActive)
                .ToListAsync(ct);

            var result = new List<ChargeCycleSummaryDto>();

            foreach (var contract in contracts)
            {
                var cycle = await _cycleRepo.GetAsync(c => c.ContractId == contract.Id && c.Period == period, ct);
                if (cycle is null)
                {
                    cycle = new ChargeCycle
                    {
                        ContractId = contract.Id,
                        Period = period,
                        Status = contract.AmountMode == AmountMode.Variable
                                       ? ChargeCycleStatus.AwaitingBill
                                       : ChargeCycleStatus.Open,
                        TotalAmount = contract.AmountMode == AmountMode.Fixed ? (contract.FixedAmount ?? 0m) : 0m,
                        DueDate = (contract.AmountMode == AmountMode.Fixed && contract.DueDay.HasValue)
                                       ? BuildDueDate(period, contract.DueDay.Value)
                                       : null
                    };

                    await _cycleRepo.AddAsync(cycle, ct);
                }

                // Approved -> funded
                var approvedForCycle = await _paymentRepo.Query()
                    .Where(p => p.ChargeId == cycle.Id && p.Status.ToString() == "Approved")
                    .ToListAsync(ct);
                cycle.FundedAmount = approvedForCycle.Sum(x => x.Tutar);

                if ((cycle.Status == ChargeCycleStatus.Open || cycle.Status == ChargeCycleStatus.Collecting)
                    && cycle.TotalAmount > 0 && cycle.FundedAmount >= cycle.TotalAmount)
                {
                    cycle.Status = ChargeCycleStatus.Funded;
                    await _cycleRepo.UpdateAsync(cycle, ct);
                }

                result.Add(new ChargeCycleSummaryDto
                {
                    Id = cycle.Id,
                    ContractId = contract.Id,
                    Type = contract.Type.ToString(),
                    AmountMode = contract.AmountMode.ToString(),
                    Period = period,
                    TotalAmount = cycle.TotalAmount,
                    PerUserShares = new List<UserShareDto>(), // TODO: house üyelerine göre doldur
                    Status = cycle.Status.ToString(),
                    BillDate = cycle.BillDate,
                    DueDate = cycle.DueDate,
                    FundedAmount = cycle.FundedAmount
                });
            }

            return new Response<List<ChargeCycleSummaryDto>>(result, true, "");
        }

        private static DateTime BuildDueDate(string period, int dueDay)
        {
            var parts = period.Split('-'); // "YYYY-MM"
            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = Math.Min(dueDay, DateTime.DaysInMonth(year, month));
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
