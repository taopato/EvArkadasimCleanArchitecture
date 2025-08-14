using System.Threading;
using System.Threading.Tasks;
using Application.Features.Charges.Commands.MarkPaid;
using Application.Features.Charges.Commands.SetBill;
using Application.Features.Charges.Dtos;
using Application.Features.Charges.Queries.GetChargesList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EvArkadasim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChargesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChargesController(IMediator mediator) => _mediator = mediator;

        // GET /api/Charges?houseId=13&period=2025-09
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int houseId, [FromQuery] string period, CancellationToken ct)
        {
            var query = new GetChargesListQuery { HouseId = houseId, Period = period };
            var res = await _mediator.Send(query, ct);
            return Ok(res);
        }

        // POST /api/Charges/{cycleId}/SetBill
        [HttpPost("{cycleId:int}/SetBill")]
        public async Task<IActionResult> SetBill([FromRoute] int cycleId, [FromBody] SetBillRequest req, CancellationToken ct)
        {
            var cmd = new SetBillCommand { CycleId = cycleId, Request = req };
            var res = await _mediator.Send(cmd, ct);
            return Ok(res);
        }

        // POST /api/Charges/{cycleId}/MarkPaid
        [HttpPost("{cycleId:int}/MarkPaid")]
        public async Task<IActionResult> MarkPaid([FromRoute] int cycleId, [FromBody] MarkPaidRequest req, CancellationToken ct)
        {
            var cmd = new MarkPaidCommand { CycleId = cycleId, Request = req };
            var res = await _mediator.Send(cmd, ct);
            return Ok(res);
        }
    }
}
