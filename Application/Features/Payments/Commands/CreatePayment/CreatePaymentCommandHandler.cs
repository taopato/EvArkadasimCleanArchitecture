using Application.Features.Payments.Commands.CreatePayment;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler
        : IRequestHandler<CreatePaymentCommand, CreatedPaymentResponseDto>
    {
        private readonly IPaymentRepository _paymentRepo;

        public CreatePaymentCommandHandler(IPaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

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
                DekontUrl = request.DekontUrl,
                OdemeTarihi = DateTime.UtcNow,     // <-- her zaman bugün/şimdi
                Aciklama = request.Aciklama ?? string.Empty, // NULL gelirse boş string yaz
                CreatedDate = DateTime.UtcNow
            };

            var created = await _paymentRepo.AddAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return new CreatedPaymentResponseDto
            {
                Id = created.Id,
                HouseId = created.HouseId,
                BorcluUserId = created.BorcluUserId,
                AlacakliUserId = created.AlacakliUserId,
                Tutar = created.Tutar,
                DekontUrl = created.DekontUrl,
                OdemeTarihi = created.OdemeTarihi,
                Aciklama = created.Aciklama
            };
        }
    }
}
