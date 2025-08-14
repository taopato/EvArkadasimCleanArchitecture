using MediatR;
using Core.Utilities.Results;

public record CreateRecurringChargeCommand(CreateRecurringChargeRequest Request)
    : IRequest<Response<RecurringChargeDto>>;
