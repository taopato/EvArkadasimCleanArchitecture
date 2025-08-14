using Application.Features.Charges.Dtos;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.Charges.Commands.SetBill
{
    public class SetBillCommand : IRequest<Response<bool>>
    {
        public int CycleId { get; set; }
        public SetBillRequest Request { get; set; } = new();
    }
}
