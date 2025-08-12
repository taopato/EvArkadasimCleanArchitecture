using System.Security.Claims;
using Application.Services.Repositories;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Payments.Commands.ApprovePayment
{
    public class ApprovePaymentCommandHandler
        : IRequestHandler<ApprovePaymentCommand, ApprovePaymentResultDto>
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IHttpContextAccessor _http;

        public ApprovePaymentCommandHandler(IPaymentRepository paymentRepo, IHttpContextAccessor http)
        {
            _paymentRepo = paymentRepo;
            _http = http;
        }

        public async Task<ApprovePaymentResultDto> Handle(ApprovePaymentCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();
            var payment = await _paymentRepo.GetByIdAsync(request.PaymentId);
            if (payment is null) throw new InvalidOperationException("Ödeme bulunamadı.");

            // Yetki: sadece alacaklı kişi onaylayabilir
            if (payment.AlacakliUserId != currentUserId)
                throw new UnauthorizedAccessException("Bu ödemeyi onaylama yetkiniz yok.");

            if (payment.Status == PaymentStatus.Approved)
                throw new InvalidOperationException("Ödeme zaten onaylanmış.");
            if (payment.Status == PaymentStatus.Rejected)
                throw new InvalidOperationException("Reddedilmiş ödeme onaylanamaz.");

            payment.Status = PaymentStatus.Approved;
            payment.ApprovedDate = DateTime.UtcNow;
            payment.ApprovedByUserId = currentUserId;

            await _paymentRepo.UpdateAsync(payment);

            // NOT: Borç/Alacak hesaplamanız Approved durumunu baz alıyorsa,
            // burada ayrıca bir denormalize güncelleme yapmaya gerek yok.
            // Denormalize bir tablo varsa: IDebtService.RecalculateForHouse(payment.HouseId);

            return new ApprovePaymentResultDto
            {
                PaymentId = payment.Id,
                ApprovedDate = payment.ApprovedDate.Value,
                ApprovedByUserId = currentUserId
            };
        }

        private int GetCurrentUserId()
        {
            var principal = _http.HttpContext?.User;
            var id =
                principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(id))
                throw new UnauthorizedAccessException("Kullanıcı doğrulanamadı.");

            return int.Parse(id);
        }
    }
}
