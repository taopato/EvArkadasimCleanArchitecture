// Application/Features/RecurringCharges/Commands/CreateRecurringCharge/CreateRecurringChargeCommandHandler.cs

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.RecurringCharges.Dtos;
using Application.Services.Repositories;
using Core.Utilities.Results;
using Domain.Entities; // ChargeType, AmountMode, SplitPolicy, RecurringCharge entity
using MediatR;

namespace Application.Features.RecurringCharges.Commands.CreateRecurringCharge
{
    public class CreateRecurringChargeCommandHandler
        : IRequestHandler<CreateRecurringChargeCommand, Response<RecurringChargeDto>>
    {
        private readonly IRecurringChargeRepository _repo;

        public CreateRecurringChargeCommandHandler(IRecurringChargeRepository repo)
        {
            _repo = repo;
        }

        public async Task<Response<RecurringChargeDto>> Handle(
            CreateRecurringChargeCommand cmd,
            CancellationToken ct)
        {
            var r = cmd.Request;

            // --- Basit doğrulamalar ---
            if (string.IsNullOrWhiteSpace(r.Type))
                return new Response<RecurringChargeDto>(null, false, "Type required");

            if (string.IsNullOrWhiteSpace(r.AmountMode))
                return new Response<RecurringChargeDto>(null, false, "AmountMode required");

            if (string.IsNullOrWhiteSpace(r.SplitPolicy))
                return new Response<RecurringChargeDto>(null, false, "SplitPolicy required");

            // Fixed/Variable kuralları
            if (r.AmountMode == "Fixed")
            {
                if (r.FixedAmount <= 0)
                    return new Response<RecurringChargeDto>(null, false, "FixedAmount > 0 required for Fixed");

                if (!r.DueDay.HasValue || r.DueDay < 1 || r.DueDay > 28)
                    return new Response<RecurringChargeDto>(null, false, "DueDay must be 1-28 for Fixed");
            }
            else if (r.AmountMode == "Variable")
            {
                // PaymentWindowDays sende non-nullable int olduğundan null kontrolü yok
                if (r.PaymentWindowDays <= 0)
                    return new Response<RecurringChargeDto>(null, false, "PaymentWindowDays > 0 required for Variable");
            }

            // SplitPolicy = Weight ise weights zorunlu
            if (r.SplitPolicy == "Weight" && (r.Weights == null || r.Weights.Count == 0))
                return new Response<RecurringChargeDto>(null, false, "Weights required for Weight split");

            // --- String -> Enum parse (PROJENDeki tipler) ---
            var typeEnum = (ChargeType)System.Enum.Parse(typeof(ChargeType), r.Type, true);
            var amountModeEnum = (AmountMode)System.Enum.Parse(typeof(AmountMode), r.AmountMode, true);
            var splitPolicyEnum = (SplitPolicy)System.Enum.Parse(typeof(SplitPolicy), r.SplitPolicy, true);

            // --- Entity oluştur ---
            var entity = new RecurringCharge
            {
                HouseId = r.HouseId,
                Type = typeEnum,                 // Domain.Entities.ChargeType
                PayerUserId = r.PayerUserId,
                AmountMode = amountModeEnum,
                SplitPolicy = splitPolicyEnum,
                FixedAmount = r.FixedAmount,            // decimal (nullable değilse de uyumlu)
                DueDay = r.DueDay,                 // int?
                PaymentWindowDays = r.PaymentWindowDays,      // int (non-nullable)
                WeightsJson = r.Weights != null ? JsonSerializer.Serialize(r.Weights) : null,
                StartMonth = r.StartMonth,
                IsActive = true                      // DTO’nda IsActive yoksa default true
            };

            entity = await _repo.AddAsync(entity, ct);

            // --- DTO çıkışı ---
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
                StartMonth = entity.StartMonth,
                IsActive = entity.IsActive
            };

            return new Response<RecurringChargeDto>(dto, true, "Recurring charge created");
        }
    }
}
