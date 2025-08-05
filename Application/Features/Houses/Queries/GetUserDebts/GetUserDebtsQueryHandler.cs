using Application.Features.Houses.Dtos;
using Application.Services.Repositories;
using Core.Utilities.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Houses.Queries.GetUserDebts
{
    public class GetUserDebtsQueryHandler : IRequestHandler<GetUserDebtsQuery, Response<GetUserDebtsDto>>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;

        public GetUserDebtsQueryHandler(
            IExpenseRepository expenseRepository,
            IUserRepository userRepository)
        {
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
        }

        public async Task<Response<GetUserDebtsDto>> Handle(GetUserDebtsQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _expenseRepository
                .Query()
                .Include(e => e.PersonalExpenses)
                .ThenInclude(p => p.User)
                .Include(e => e.UserOdeyen)
                .Where(e => e.HouseId == request.HouseId)
                .ToListAsync(cancellationToken);

            var dto = new GetUserDebtsDto
            {
                UserId = request.UserId,
                HouseId = request.HouseId
            };

            foreach (var e in expenses)
            {
                var paylasimTutari = e.OrtakHarcamaTutari / Math.Max(1, e.PersonalExpenses.Count);

                // Borç: Kullanıcı ortak harcamaya dahilse
                if (e.PersonalExpenses.Any(pe => pe.UserId == request.UserId))
                {
                    dto.ToplamBorc += paylasimTutari;
                    dto.Detaylar.Add(new UserDebtDetailDto
                    {
                        ExpenseId = e.Id,
                        Tur = e.Description,
                        Tutar = e.Tutar,
                        OdeyenUserId = e.OdeyenUserId,
                        OdeyenKullaniciAdi = $"{e.UserOdeyen?.FirstName} {e.UserOdeyen?.LastName}",
                        PaylasimTutari = paylasimTutari
                    });
                }

                // Alacak: Kullanıcı harcamayı ödeyen kişi ise
                if (e.OdeyenUserId == request.UserId)
                {
                    dto.ToplamAlacak += e.OrtakHarcamaTutari;
                }
            }

            return new Response<GetUserDebtsDto>(dto, true);
        }
    }
}
