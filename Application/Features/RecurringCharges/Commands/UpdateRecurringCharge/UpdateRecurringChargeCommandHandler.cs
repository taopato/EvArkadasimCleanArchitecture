using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.RecurringCharges.Dtos;
using Application.Services.Repositories;
using Core.Utilities.Results;
using Domain.Entities;
using Domain.Enums; // 🔴 SplitPolicy vb.
using MediatR;

namespace Application.Features.RecurringCharges.Commands.UpdateRecurringCharge
{
    public class UpdateRecurringChargeCommandHandler
        : IRequestHandler<UpdateRecurringChargeCommand, Response<RecurringChargeDto>>
    {
        private readonly IRecurringChargeRepository _repo;

        public UpdateRecurringChargeCommandHandler(IRecurringChargeRepository repo)
        {
            _repo = repo;
        }

        public async Task<Response<RecurringChargeDto>> Handle(UpdateRecurringChargeCommand cmd, CancellationToken ct)
        {
            var entity = await _repo.GetAsync(x => x.Id == cmd.Id, ct);
            if (entity is null)
                return new Response<RecurringChargeDto>(null, false, "Recurring charge not found");

            var r = cmd.Request;

            // Split policy & weights
            if (!string.IsNullOrWhiteSpace(r.SplitPolicy))
            {
                if (r.SplitPolicy is "Equal" or "Weight")
                    entity.SplitPolicy = (SplitPolicy)System.Enum.Parse(typeof(SplitPolicy), r.SplitPolicy, true);
                else
                    return new Response<RecurringChargeDto>(null, false, "Invalid SplitPolicy");
            }

            if (entity.SplitPolicy == SplitPolicy.Weight)
            {
                if (r.Weights == null || r.Weights.Count == 0)
                    return new Response<RecurringChargeDto>(null, false, "Weights required for Weight split");
                entity.WeightsJson = JsonSerializer.Serialize(r.Weights);
            }
            else
            {
                if (r.Weights != null)
                    entity.WeightsJson = JsonSerializer.Serialize(r.Weights);
            }

            // Fixed alanları
            if (r.FixedAmount.HasValue) entity.FixedAmount = r.FixedAmount.Value;
            if (r.DueDay.HasValue)
            {
                if (r.DueDay < 1 || r.DueDay > 28)
                    return new Response<RecurringChargeDto>(null, false, "DueDay must be 1-28");
                entity.DueDay = r.DueDay;
            }

            // Variable alanları
            if (r.PaymentWindowDays.HasValue)
            {
                if (r.PaymentWindowDays <= 0)
                    return new Response<RecurringChargeDto>(null, false, "PaymentWindowDays must be > 0");
                entity.PaymentWindowDays = r.PaymentWindowDays.Value;
            }

            // Payer ve aktiflik
            if (r.PayerUserId > 0) entity.PayerUserId = r.PayerUserId;
            // DTO'nda IsActive var ise aşağıyı açabilirsin; yoksa dokunma
            // entity.IsActive = r.IsActive;

            await _repo.UpdateAsync(entity, ct);

            var dto = new RecurringChargeDto
            {
                Id = entity.Id,
                HouseId = entity.HouseId,
                Type = entity.Type.ToString(),
                PayerUserId = entity.PayerUserId,
                AmountMode = entity.AmountMode.ToString(),
                SplitPolicy = entity.SplitPolicy.ToString(),
                FixedAmount = entity.FixedAmount,
                DueDay = entity.DueDay,
                PaymentWindowDays = entity.PaymentWindowDays,
                // DTO'nda yoksa yazma:
                // WeightsJson    = entity.WeightsJson,
                StartMonth = entity.StartMonth,
                IsActive = entity.IsActive
            };

            return new Response<RecurringChargeDto>(dto, true, "Recurring charge updated");
        }
    }
}
