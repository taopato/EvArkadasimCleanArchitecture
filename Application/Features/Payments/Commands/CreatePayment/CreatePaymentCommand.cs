using MediatR;
using Application.Features.Payments.Dtos;

namespace Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommand : IRequest<CreatedPaymentResponseDto>
    {
        public int HouseId { get; set; }
        public int BorcluUserId { get; set; }
        public int AlacakliUserId { get; set; }
        public decimal Tutar { get; set; }
    }
}