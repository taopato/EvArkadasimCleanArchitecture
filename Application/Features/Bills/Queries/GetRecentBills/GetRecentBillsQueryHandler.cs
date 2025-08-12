using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Application.Services.Repositories;
using Application.Features.Bills.Dtos;

namespace Application.Features.Bills.Queries.GetRecentBills
{
    public class GetRecentBillsQueryHandler : IRequestHandler<GetRecentBillsQuery, List<BillDetailDto>>
    {
        private readonly IUtilityBillRepository _billRepo; // <--- DİKKAT
        public GetRecentBillsQueryHandler(IUtilityBillRepository billRepo) // <--- DİKKAT
        {
            _billRepo = billRepo;
        }

        public async Task<List<BillDetailDto>> Handle(GetRecentBillsQuery request, CancellationToken ct)
        {
            var list = await _billRepo.GetRecentAsync(request.HouseId, request.UtilityType, request.Limit);
            return list.Select(b => new BillDetailDto
            {
                BillId = b.Id,
                HouseId = b.HouseId,
                UtilityType = b.UtilityType,
                Month = b.Month,
                ResponsibleUserId = b.ResponsibleUserId,
                Amount = b.Amount,
                DueDate = b.DueDate,
                Note = b.Note,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt,
                FinalizedAt = b.FinalizedAt,
                Shares = b.Shares.Select(s => new BillDetailDto.BillShareItem { UserId = s.UserId, Amount = s.ShareAmount }).ToList(),
                Documents = b.Documents.Select(d => new BillDetailDto.BillDocumentItem
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    FileUrl = d.FilePathOrUrl,
                    UploadedAt = d.UploadedAt
                }).ToList()
            }).ToList();
        }
    }
}
