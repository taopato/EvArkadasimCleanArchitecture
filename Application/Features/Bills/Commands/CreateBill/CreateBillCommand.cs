using MediatR;
using Application.Features.Bills.Dtos;

namespace Application.Features.Bills.Commands.CreateBill
{
    public class CreateBillCommand : IRequest<BillDetailDto>
    {
        public CreateBillDto Model { get; set; } = null!;
    }
}
