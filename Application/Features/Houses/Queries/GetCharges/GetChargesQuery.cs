using MediatR;
using System.Collections.Generic;

namespace Application.Features.Expenses.Queries.GetCharges
{
    public class GetChargesQuery : IRequest<List<ChargeCycleDto>>
    {
        public int HouseId { get; set; }
        public string? Period { get; set; } // "YYYY-MM" veya null
    }

    public class ChargeCycleDto
    {
        public int CycleId { get; set; }
        public int ContractId { get; set; }
        public int HouseId { get; set; }
        public string Period { get; set; } = "";
        public int Status { get; set; }             // enum int
        public decimal TotalAmount { get; set; }
        public decimal FundedAmount { get; set; }
        public int Type { get; set; }               // enum int
        public int SplitPolicy { get; set; }        // enum int
        public decimal? FixedAmount { get; set; }
        public int PayerUserId { get; set; }
    }
}
