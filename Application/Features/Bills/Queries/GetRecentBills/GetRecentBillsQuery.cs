using MediatR;
using System.Collections.Generic;
using Application.Features.Bills.Dtos;
using Domain.Enums;

namespace Application.Features.Bills.Queries.GetRecentBills
{
    public class GetRecentBillsQuery : IRequest<List<BillDetailDto>>
    {
        public int HouseId { get; set; }
        public UtilityType? UtilityType { get; set; }
        public int Limit { get; set; } = 10;
    }
}
