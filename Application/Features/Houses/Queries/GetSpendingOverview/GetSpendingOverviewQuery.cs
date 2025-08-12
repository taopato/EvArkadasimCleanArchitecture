using System;
using MediatR;

namespace Application.Features.Houses.Queries.GetSpendingOverview
{
    public class GetSpendingOverviewQuery : IRequest<SpendingOverviewDto>
    {
        public int HouseId { get; set; }
        public DateTime? From { get; set; } // verilmezse son 90 gün
        public DateTime? To { get; set; }
        public int RecentLimit { get; set; } = 10;
    }
}
