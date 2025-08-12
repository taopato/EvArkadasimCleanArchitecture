using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Features.Bills.Dtos;
using Application.Services.Repositories;
using Domain.Entities;

namespace Application.Features.Bills.Commands.CreateBill
{
    public class CreateBillCommandHandler : IRequestHandler<CreateBillCommand, BillDetailDto>
    {
        private readonly IUtilityBillRepository _billRepo;
        private readonly IHouseMemberRepository _houseMemberRepo;

        public CreateBillCommandHandler(IUtilityBillRepository billRepo, IHouseMemberRepository houseMemberRepo)
        {
            _billRepo = billRepo;
            _houseMemberRepo = houseMemberRepo;
        }

        public async Task<BillDetailDto> Handle(CreateBillCommand request, CancellationToken ct)
        {
            var m = request.Model;

            var exists = await _billRepo.GetByHouseMonthAsync(m.HouseId, m.UtilityType, m.Month);
            if (exists != null)
                throw new System.Exception("Bu ay için aynı türde fatura zaten mevcut.");

            var bill = new Bill
            {
                HouseId = m.HouseId,
                UtilityType = m.UtilityType,
                Month = m.Month,
                ResponsibleUserId = m.ResponsibleUserId,
                Amount = m.Amount,
                DueDate = m.DueDate,
                Note = m.Note,
                CreatedByUserId = m.CreatedByUserId
            };

            var memberIds = await _houseMemberRepo.GetActiveUserIdsAsync(m.HouseId, ct);
            if (memberIds == null || memberIds.Count == 0)
                throw new System.Exception("Aktif ev üyesi bulunamadı.");

            var count = memberIds.Count;

            var baseShare = decimal.Round(m.Amount / count, 2, System.MidpointRounding.AwayFromZero);
            var totalShares = baseShare * count;
            var diff = decimal.Round(m.Amount - totalShares, 2, System.MidpointRounding.AwayFromZero);

            foreach (var uid in memberIds)
                bill.Shares.Add(new BillShare { UserId = uid, ShareAmount = baseShare });

            if (diff != 0m)
            {
                var respShare = bill.Shares.First(s => s.UserId == m.ResponsibleUserId);
                respShare.ShareAmount = decimal.Round(respShare.ShareAmount + diff, 2, System.MidpointRounding.AwayFromZero);
            }

            bill = await _billRepo.AddAsync(bill);

            return new BillDetailDto
            {
                BillId = bill.Id,
                HouseId = bill.HouseId,
                UtilityType = bill.UtilityType,
                Month = bill.Month,
                ResponsibleUserId = bill.ResponsibleUserId,
                Amount = bill.Amount,
                DueDate = bill.DueDate,
                Note = bill.Note,
                Status = bill.Status.ToString(),
                CreatedAt = bill.CreatedAt,
                Shares = bill.Shares.Select(s => new BillDetailDto.BillShareItem
                {
                    UserId = s.UserId,
                    Amount = s.ShareAmount
                }).ToList()
            };
        }
    }
}
