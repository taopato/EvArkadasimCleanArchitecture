﻿using MediatR;
using Application.Features.Expenses.Dtos;
using Application.Services.Repositories;
using AutoMapper;

namespace Application.Features.Expenses.Queries.GetExpensesByHouse
{
    public class GetExpensesByHouseQueryHandler
        : IRequestHandler<GetExpensesByHouseQuery, List<ExpenseListDto>>
    {
        private readonly IExpenseRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;

        public GetExpensesByHouseQueryHandler(
            IExpenseRepository repo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<List<ExpenseListDto>> Handle(
            GetExpensesByHouseQuery request,
            CancellationToken ct)
        {
            var list = await _repo.GetByHouseIdAsync(request.HouseId);
            var dtoList = new List<ExpenseListDto>();
            foreach (var e in list)
            {
                // 🔹 Soft-deleted olanları listeleme
                if (!e.IsActive) continue;

                var dto = _mapper.Map<ExpenseListDto>(e);
                var payer = await _userRepo.GetByIdAsync(e.OdeyenUserId);
                var creator = await _userRepo.GetByIdAsync(e.KaydedenUserId);

                dto.OdeyenKullaniciAdi = payer is null ? "" : $"{payer.FirstName} {payer.LastName}";
                dto.KaydedenKullaniciAdi = creator is null ? "" : $"{creator.FirstName} {creator.LastName}";
                dto.KayitTarihi = e.CreatedDate;

                // 🔹 Notu description'a yansıt
                if (string.IsNullOrWhiteSpace(dto.Description))
                    dto.Description = e.Note ?? string.Empty;

                dtoList.Add(dto);
            }
            return dtoList;
        }
    }
}
