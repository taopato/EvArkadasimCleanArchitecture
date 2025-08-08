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

        public GetUserDebtsQueryHandler(
            IExpenseRepository expenseRepo,
            IUserRepository userRepo)
        {
            _expenseRepo = expenseRepo;
            _userRepo = userRepo;
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

            // 2) Aktif harcamaları getir (IsActive eklediyseniz; yoksa sadece HouseId filtresi)
            var expenses = await _expenseRepo.Query()
                .Where(e => e.HouseId == request.HouseId /*&& e.IsActive*/)
                .Include(e => e.Shares)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // 3) UserId → FullName sözlüğü
            var userDict = await _userRepo.GetAllUserDictionaryAsync();

            // 4) (debtor, creditor) → tutar matriksi
            var pairDebts = new Dictionary<(int debtor, int creditor), decimal>();

            foreach (var exp in expenses)
            {
                var creditor = exp.OdeyenUserId;

                foreach (var share in exp.Shares)
                {
                    var debtor = share.UserId;
                    if (debtor == creditor)
                        continue; // kendi payı

                    var key = (debtor, creditor);
                    if (!pairDebts.ContainsKey(key))
                        pairDebts[key] = 0m;

                    // *** Burada mutlaka doğru alanı kullanıyoruz! ***
                    pairDebts[key] += share.PaylasimTutar;

                    // detay olarak ekleyeceksek:
                    if (debtor == request.UserId || creditor == request.UserId)
                    {
                        dto.Detaylar.Add(new UserDebtDetailDto
                        {
                            ExpenseId = exp.Id,
                            Tur = exp.Description,
                            Tutar = exp.Tutar,
                            OdeyenUserId = creditor,
                            OdeyenKullaniciAdi = userDict[creditor],
                            PaylasimTutar = share.PaylasimTutar
                        });
                    }
                }
            }

            // 5) Her karşı tarafla net durumu hesapla
            var others = pairDebts.Keys
                .SelectMany(k => new[] { k.debtor, k.creditor })
                .Distinct()
                .Where(u => u != request.UserId);

            foreach (var other in others)
            {
                var aOwesB = pairDebts.GetValueOrDefault((request.UserId, other), 0m);
                var bOwesA = pairDebts.GetValueOrDefault((other, request.UserId), 0m);

                var net = bOwesA - aOwesB;
                if (net == 0m)
                    continue;

                dto.KullaniciBazliDurumlar.Add(new KullaniciBazliDurumDto
                {
                    UserId = other,
                    FullName = userDict[other],
                    Amount = net
                });
            }

            // 6) Toplam borç / alacak
            dto.ToplamAlacak = dto.KullaniciBazliDurumlar
                .Where(x => x.Amount > 0)
                .Sum(x => x.Amount);

            dto.ToplamBorc = -dto.KullaniciBazliDurumlar
                .Where(x => x.Amount < 0)
                .Sum(x => x.Amount);

            // 7) Yanıtı dön
            return new Response<GetUserDebtsDto>(dto, true, string.Empty);

        }
    }
}
