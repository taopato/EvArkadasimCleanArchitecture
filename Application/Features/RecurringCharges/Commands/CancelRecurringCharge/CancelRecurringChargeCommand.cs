using Application.Features.RecurringCharges.Dtos;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.RecurringCharges.Commands.CancelRecurringCharge
{
    public class CancelRecurringChargeCommand : IRequest<Response<bool>>
    {
        public int Id { get; }
        public CancelRecurringChargeRequest Request { get; }

        public CancelRecurringChargeCommand(int id, CancelRecurringChargeRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
