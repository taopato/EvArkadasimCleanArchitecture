using MediatR;
using Domain.Entities;
using Application.Services.Repositories;
using Core.Utilities.Results;

public class CreateRecurringChargeCommandHandler
    : IRequestHandler<CreateRecurringChargeCommand, Response<RecurringChargeDto>>
{
    private readonly IRecurringChargeRepository _repo;

    public CreateRecurringChargeCommandHandler(IRecurringChargeRepository repo)
    {
        _repo = repo;
    }

    public async Task<Response<RecurringChargeDto>> Handle(
        CreateRecurringChargeCommand cmd, CancellationToken ct)
    {
        var r = cmd.Request;

        var contract = new RecurringCharge
        {
            HouseId = r.HouseId,
            Type = Enum.Parse<ChargeType>(r.Type, true),
            PayerUserId = r.PayerUserId,
            AmountMode = Enum.Parse<AmountMode>(r.AmountMode, true),
            FixedAmount = r.FixedAmount,
            SplitPolicy = Enum.Parse<SplitPolicy>(r.SplitPolicy, true),
            WeightsJson = r.Weights is null ? null : System.Text.Json.JsonSerializer.Serialize(r.Weights),
            DueDay = r.DueDay,
            PaymentWindowDays = r.PaymentWindowDays,
            StartMonth = r.StartMonth,
            IsActive = true
        };

        if (contract.AmountMode == AmountMode.Fixed && contract.FixedAmount is null)
            return new Response<RecurringChargeDto>(null!, false, "FixedAmount zorunlu");
        if (contract.AmountMode == AmountMode.Variable && contract.FixedAmount is not null)
            contract.FixedAmount = null;

        var added = await _repo.AddAsync(contract, ct);

        var dto = new RecurringChargeDto
        {
            Id = added.Id,
            HouseId = added.HouseId,
            Type = added.Type.ToString(),
            PayerUserId = added.PayerUserId,
            AmountMode = added.AmountMode.ToString(),
            FixedAmount = added.FixedAmount,
            SplitPolicy = added.SplitPolicy.ToString(),
            DueDay = added.DueDay,
            PaymentWindowDays = added.PaymentWindowDays,
            StartMonth = added.StartMonth,
            IsActive = added.IsActive
        };

        return new Response<RecurringChargeDto>(dto, true, "Recurring charge created");
    }
}
