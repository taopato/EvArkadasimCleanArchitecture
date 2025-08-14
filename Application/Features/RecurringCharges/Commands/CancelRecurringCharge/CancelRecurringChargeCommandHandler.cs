using System.Threading;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.RecurringCharges.Commands.CancelRecurringCharge
{
    public class CancelRecurringChargeCommandHandler
        : IRequestHandler<CancelRecurringChargeCommand, Response<bool>>
    {
        private readonly IRecurringChargeRepository _repo;

        public CancelRecurringChargeCommandHandler(IRecurringChargeRepository repo)
        {
            _repo = repo;
        }

        public async Task<Response<bool>> Handle(CancelRecurringChargeCommand cmd, CancellationToken ct)
        {
            var entity = await _repo.GetAsync(x => x.Id == cmd.Id, ct);
            if (entity is null)
                return new Response<bool>(false, false, "Recurring charge not found");

            // Soft cancel: isActive=false
            entity.IsActive = false;

            // Opsiyonel: ileri tarihli etkisi için alan tutuyorsan set edebilirsin
            // örn. entity.CancelEffectiveFrom = cmd.Request.EffectiveFromPeriod;

            await _repo.UpdateAsync(entity, ct);
            return new Response<bool>(true, true, "Recurring charge canceled");
        }
    }
}
