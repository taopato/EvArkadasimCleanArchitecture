using Application.Features.Charges.Dtos;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.Charges.Commands.MarkPaid
{
    public class MarkPaidCommand : IRequest<Response<bool>>
    {
        public int CycleId { get; set; }
        public MarkPaidRequest Request { get; set; } = new();
    }
}
