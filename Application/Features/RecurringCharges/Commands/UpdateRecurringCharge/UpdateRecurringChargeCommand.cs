using Application.Features.RecurringCharges.Dtos;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.RecurringCharges.Commands.UpdateRecurringCharge
{
    public class UpdateRecurringChargeCommand : IRequest<Response<RecurringChargeDto>>
    {
        public int Id { get; }
        public UpdateRecurringChargeRequest Request { get; }

        public UpdateRecurringChargeCommand(int id, UpdateRecurringChargeRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
