using System.Threading;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Core.Utilities.Results;
using Domain.Entities;
using MediatR;

namespace Application.Features.Charges.Commands.SetBill
{
    public class SetBillCommandHandler : IRequestHandler<SetBillCommand, Response<bool>>
    {
        private readonly IChargeCycleRepository _cycleRepo;
        private readonly IRecurringChargeRepository _recRepo;

        public SetBillCommandHandler(IChargeCycleRepository cycleRepo, IRecurringChargeRepository recRepo)
        {
            _cycleRepo = cycleRepo;
            _recRepo = recRepo;
        }

        public async Task<Response<bool>> Handle(SetBillCommand cmd, CancellationToken ct)
        {
            var cycle = await _cycleRepo.GetAsync(c => c.Id == cmd.CycleId, ct);
            if (cycle is null) return new Response<bool>(false, false, "Cycle not found");

            var contract = await _recRepo.GetAsync(r => r.Id == cycle.ContractId, ct);
            if (contract is null) return new Response<bool>(false, false, "Contract not found");

            if (contract.AmountMode != AmountMode.Variable)
                return new Response<bool>(false, false, "Only variable charges can set bill");

            var r = cmd.Request;
            if (r.TotalAmount <= 0m) return new Response<bool>(false, false, "TotalAmount must be > 0");
            if (string.IsNullOrWhiteSpace(r.BillDocumentUrl))
                return new Response<bool>(false, false, "BillDocumentUrl required");

            cycle.TotalAmount = r.TotalAmount;
            cycle.BillDate = r.BillDate;
            cycle.BillNumber = r.BillNumber;
            cycle.BillDocumentUrl = r.BillDocumentUrl;
            cycle.DueDate = r.BillDate.AddDays(contract.PaymentWindowDays);
            cycle.Status = ChargeCycleStatus.Collecting;

            await _cycleRepo.UpdateAsync(cycle, ct);
            return new Response<bool>(true, true, "Bill set");
        }
    }
}
