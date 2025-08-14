// Application/Features/Houses/Queries/GetUserDebts/GetUserDebtsQueryHandler.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Houses.Dtos;
using Application.Services.Repositories;
using Core.Utilities.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Houses.Queries.GetUserDebts
{
    public class GetUserDebtsQueryHandler
        : IRequestHandler<GetUserDebtsQuery, Response<GetUserDebtsDto>>
    {
        private readonly IExpenseRepository _expenseRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPaymentRepository _paymentRepo;

        public GetUserDebtsQueryHandler(
            IExpenseRepository expenseRepo,
            IUserRepository userRepo,
            IPaymentRepository paymentRepo)
        {
            _expenseRepo = expenseRepo;
            _userRepo = userRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<Response<GetUserDebtsDto>> Handle(
            GetUserDebtsQuery request,
            CancellationToken cancellationToken)
        {
            // 1) Başlangıç DTO
            var dto = new GetUserDebtsDto
            {
                UserId = request.UserId,
                HouseId = request.HouseId
            };

            // 2) Harcamalar (IsActive filtreyi kullanıyorsan aç)
            var expenses = await _expenseRepo.Query()
                .Where(e => e.HouseId == request.HouseId /*&& e.IsActive*/)
                .Include(e => e.Shares)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // 2.5) Ödemeler: önce materialize et, sonra Approved filtrele (enum/string güvenli)
            var paymentsAll = await _paymentRepo.Query()
                .Where(p => p.HouseId == request.HouseId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var approvedPayments = paymentsAll
                .Where(p => p.Status != null && p.Status.ToString() == "Approved")
                .ToList();

            // 3) User sözlüğü
            var userDict = await _userRepo.GetAllUserDictionaryAsync();

            // 4) (debtor, creditor) → tutar matriksi
            var pairDebts = new Dictionary<(int debtor, int creditor), decimal>();

            // 4.a) Harcamalardan doğan borç: borçlu → ödeyen
            foreach (var exp in expenses)
            {
                var creditor = exp.OdeyenUserId; // ödeyen

                foreach (var share in exp.Shares)
                {
                    var debtor = share.UserId;    // pay sahibi
                    if (debtor == creditor) continue; // ödeyen kendi payı için borçlanmaz

                    var key = (debtor, creditor);
                    if (!pairDebts.ContainsKey(key))
                        pairDebts[key] = 0m;

                    pairDebts[key] += share.PaylasimTutar;

                    // İlgili kullanıcı ise detay ekle
                    if (debtor == request.UserId || creditor == request.UserId)
                    {
                        userDict.TryGetValue(creditor, out var creditorName);
                        dto.Detaylar.Add(new UserDebtDetailDto
                        {
                            ExpenseId = exp.Id,
                            Tur = exp.Description,   // sende alan adı farklıysa uyarlayabilirsin
                            Tutar = exp.Tutar,
                            OdeyenUserId = creditor,
                            OdeyenKullaniciAdi = creditorName ?? creditor.ToString(),
                            PaylasimTutar = share.PaylasimTutar
                        });
                    }
                }
            }

            // 4.b) ONAYLANMIŞ ÖDEMELERİ borçtan düş: (borçlu, alacaklı) kenarı azalır
            foreach (var pay in approvedPayments)
            {
                // Alanlar int (nullable değil)
                int debtor = pay.BorcluUserId;
                int creditor = pay.AlacakliUserId;

                var key = (debtor, creditor);
                if (!pairDebts.ContainsKey(key))
                    pairDebts[key] = 0m;

                pairDebts[key] -= pay.Tutar; // ödeme kadar borç düş
            }

            // 5) Bu kullanıcıya göre net ( + = alacak, - = borç )
            var others = pairDebts.Keys
                .SelectMany(k => new[] { k.debtor, k.creditor })
                .Distinct()
                .Where(u => u != request.UserId);

            foreach (var other in others)
            {
                var aOwesB = pairDebts.GetValueOrDefault((request.UserId, other), 0m); // ben → other
                var bOwesA = pairDebts.GetValueOrDefault((other, request.UserId), 0m); // other → ben

                var net = bOwesA - aOwesB; // + ise alacaklıyım, - ise borçluyum
                if (net == 0m) continue;

                userDict.TryGetValue(other, out var name);
                dto.KullaniciBazliDurumlar.Add(new KullaniciBazliDurumDto
                {
                    UserId = other,
                    FullName = name ?? other.ToString(),
                    Amount = net
                });
            }

            // 6) Toplamlar
            dto.ToplamAlacak = dto.KullaniciBazliDurumlar
                .Where(x => x.Amount > 0)
                .Sum(x => x.Amount);

            dto.ToplamBorc = -dto.KullaniciBazliDurumlar
                .Where(x => x.Amount < 0)
                .Sum(x => x.Amount);

            // NetDurum read-only ise set etmiyoruz (DTO hesaplıyorsa otomatik)

            // 7) Response
            return new Response<GetUserDebtsDto>(dto, true, string.Empty);
        }
    }
}
