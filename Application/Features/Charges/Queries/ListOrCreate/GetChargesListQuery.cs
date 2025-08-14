using System.Collections.Generic;
using Application.Features.Charges.Dtos;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.Charges.Queries.GetChargesList
{
    // Parametresiz class; Controller object-initializer ile doldurur.
    public class GetChargesListQuery : IRequest<Response<List<ChargeCycleSummaryDto>>>
    {
        public int HouseId { get; set; }
        public string Period { get; set; } = ""; // "YYYY-MM"
    }
}
