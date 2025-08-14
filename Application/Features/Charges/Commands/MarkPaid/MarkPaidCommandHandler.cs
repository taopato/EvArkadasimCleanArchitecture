using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Core.Utilities.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Charges.Commands.MarkPaid
{
    public class MarkPaidCommandHandler : IRequestHandler<MarkPaidCommand, Response<bool>>
    {
        private readonly IChargeCycleRepository _cycleRepo;
        private readonly IPaymentRepository _paymentRepo;

        public MarkPaidCommandHandler(IChargeCycleRepository cycleRepo, IPaymentRepository paymentRepo)
        {
            _cycleRepo = cycleRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<Response<bool>> Handle(MarkPaidCommand cmd, CancellationToken ct)
        {
            var cycle = await _cycleRepo.GetAsync(c => c.Id == cmd.CycleId, ct);
            if (cycle is null) return new Response<bool>(false, false, "Cycle not found");

            if (cycle.Status == ChargeCycleStatus.Paid)
                return new Response<bool>(false, false, "Already paid");

            var approved = await _paymentRepo.Query()
                .Where(p => p.ChargeId == cycle.Id && p.Status.ToString() == "Approved")
                .ToListAsync(ct);

            var funded = approved.Sum(x => x.Tutar);
            if (funded < cycle.TotalAmount)
                return new Response<bool>(false, false, "Insufficient funded amount");

            var r = cmd.Request;
            if (string.IsNullOrWhiteSpace(r.ExternalReceiptUrl))
                return new Response<bool>(false, false, "Receipt required");

            cycle.FundedAmount = funded;
            cycle.PaidDate = r.PaidDate;
            cycle.ExternalReceiptUrl = r.ExternalReceiptUrl;
            cycle.Status = ChargeCycleStatus.Paid;

            await _cycleRepo.UpdateAsync(cycle, ct);
            return new Response<bool>(true, true, "Marked as paid");
        }
    }
}
