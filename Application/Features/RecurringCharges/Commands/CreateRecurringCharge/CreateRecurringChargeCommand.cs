using Application.Features.RecurringCharges.Dtos;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.RecurringCharges.Commands.CreateRecurringCharge
{
    public class CreateRecurringChargeCommand : IRequest<Response<RecurringChargeDto>>
    {
        public CreateRecurringChargeRequest Request { get; }
        public CreateRecurringChargeCommand(CreateRecurringChargeRequest req) => Request = req;
    }
}
