using MediatR;
using Application.Features.Payments.Dtos;
using Application.Services.Repositories;
using Domain.Entities;

namespace Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler
        : IRequestHandler<CreatePaymentCommand, CreatedPaymentResponseDto>
    {
        private readonly IPaymentRepository _paymentRepo;

        public CreatePaymentCommandHandler(IPaymentRepository paymentRepo)
            => _paymentRepo = paymentRepo;

        public async Task<CreatedPaymentResponseDto> Handle(
            CreatePaymentCommand request,
            CancellationToken cancellationToken)
        {
            var payment = new Payment
            {
                HouseId = request.HouseId,
                BorcluUserId = request.BorcluUserId,
                AlacakliUserId = request.AlacakliUserId,
                Tutar = request.Tutar,
                CreatedDate = DateTime.UtcNow
            };
            var added = await _paymentRepo.AddAsync(payment);
            return new CreatedPaymentResponseDto { Id = added.Id };
        }
    }
}