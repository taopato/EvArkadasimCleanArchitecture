using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Repositories;
using MediatR;

namespace Application.Features.Payments.Queries.GetPendingPayments
{
    public class GetPendingPaymentsQueryHandler
        : IRequestHandler<GetPendingPaymentsQuery, IList<PendingPaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepo;

        public GetPendingPaymentsQueryHandler(IPaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        public async Task<IList<PendingPaymentDto>> Handle(GetPendingPaymentsQuery request, CancellationToken cancellationToken)
        {
            var list = await _paymentRepo.GetPendingByAlacakliAsync(request.UserId);

            return list.Select(p => new PendingPaymentDto
            {
                Id = p.Id,
                HouseId = p.HouseId,
                BorcluUserId = p.BorcluUserId,
                AlacakliUserId = p.AlacakliUserId,
                BorcluUserName = p.BorcluUser != null
                    ? $"{p.BorcluUser.FirstName} {p.BorcluUser.LastName}".Trim()
                    : string.Empty,
                Tutar = p.Tutar,
                PaymentMethod = p.PaymentMethod.ToString(),
                OdemeTarihi = p.OdemeTarihi,
                Aciklama = p.Aciklama,
                DekontUrl = p.DekontUrl,
                Status = p.Status.ToString()
            }).ToList();
        }
    }
}
